// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Shell;

namespace Thumbico;

/// <summary>
/// Asks the Windows shell to render a shell item as a bitmap.
/// </summary>
internal static class ShellImage
{
    /// <summary>
    /// Renders the shell item at <paramref name="path"/> and converts the result to a managed bitmap.
    /// </summary>
    /// <param name="isIcon">Receives true when the shell returned an icon rather than a thumbnail.</param>
    internal static Bitmap Get(string path, Size size, ThumbicoSource source, ThumbicoOptions options, out bool isIcon)
    {
        PInvoke.SHCreateItemFromParsingName(path, null, out IShellItemImageFactory factory).ThrowOnFailure();

        SIZE requested = new() { cx = size.Width, cy = size.Height };
        SIIGBF flags = ToNativeFlags(options);
        Bitmap bitmap;

        // Auto tries a thumbnail first, then falls back to the icon for items that have none.
        if (source == ThumbicoSource.Auto)
        {
            if (TryGetBitmap(factory, requested, flags | SIIGBF.SIIGBF_THUMBNAILONLY, out Bitmap? thumbnail))
            {
                isIcon = false;
                bitmap = thumbnail;
            }
            else
            {
                isIcon = true;
                bitmap = GetBitmap(factory, requested, flags | SIIGBF.SIIGBF_ICONONLY);
            }
        }
        else
        {
            isIcon = source == ThumbicoSource.IconOnly;
            SIIGBF sourceFlag = isIcon ? SIIGBF.SIIGBF_ICONONLY : SIIGBF.SIIGBF_THUMBNAILONLY;
            bitmap = GetBitmap(factory, requested, flags | sourceFlag);
        }

        // Icons come back bottom-up and thumbnails top-down, while the DIB header reports a positive
        // height for both - so the kind returned is the only signal that distinguishes them.
        if (isIcon)
        {
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        return bitmap;
    }

    private static bool TryGetBitmap(
        IShellItemImageFactory factory,
        SIZE size,
        SIIGBF flags,
        [NotNullWhen(true)] out Bitmap? bitmap)
    {
        try
        {
            bitmap = GetBitmap(factory, size, flags);
            return true;
        }
        catch (COMException)
        {
            bitmap = null;
            return false;
        }
    }

    private static Bitmap GetBitmap(IShellItemImageFactory factory, SIZE size, SIIGBF flags)
    {
        factory.GetImage(size, flags, out DeleteObjectSafeHandle hbitmap);
        using (hbitmap)
        {
            return ToManagedBitmap(hbitmap);
        }
    }

    /// <summary>
    /// Copies a native bitmap into a managed one that owns its pixels, preserving transparency.
    /// </summary>
    /// <remarks>
    /// Image.FromHbitmap is documented not to preserve the alpha channel, which icons depend on, so the
    /// DIB section is read directly instead.
    /// </remarks>
    private static unsafe Bitmap ToManagedBitmap(DeleteObjectSafeHandle hbitmap)
    {
        BITMAP info = default;
        Span<byte> buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref info, 1));
        int copied = PInvoke.GetObject(hbitmap, buffer);

        // Below 32 bits per pixel there is no alpha to preserve, and the stride assumed below would not
        // match the rows.
        if (copied < buffer.Length || info.bmBitsPixel != 32)
        {
            return Image.FromHbitmap(hbitmap.DangerousGetHandle());
        }

        // The first borrows the shell's pixels, the second owns them - which is what lets the result
        // outlive the native bitmap the caller frees on return.
        using Bitmap borrowed = new(
            info.bmWidth, info.bmHeight, info.bmWidthBytes, PixelFormat.Format32bppArgb, (nint)info.bmBits);

        Bitmap owned = new(info.bmWidth, info.bmHeight, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(owned))
        {
            // SourceCopy, or the default blends into the transparent canvas and alters the alpha.
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.DrawImageUnscaled(borrowed, 0, 0);
        }

        return owned;
    }

    private static SIIGBF ToNativeFlags(ThumbicoOptions options)
    {
        SIIGBF flags = SIIGBF.SIIGBF_RESIZETOFIT;

        if (options.HasFlag(ThumbicoOptions.AllowLargerSize))
        {
            flags |= SIIGBF.SIIGBF_BIGGERSIZEOK;
        }

        if (options.HasFlag(ThumbicoOptions.InMemoryOnly))
        {
            flags |= SIIGBF.SIIGBF_MEMORYONLY;
        }

        if (options.HasFlag(ThumbicoOptions.InCacheOnly))
        {
            flags |= SIIGBF.SIIGBF_INCACHEONLY;
        }

        if (options.HasFlag(ThumbicoOptions.CropToSquare))
        {
            flags |= SIIGBF.SIIGBF_CROPTOSQUARE;
        }

        if (options.HasFlag(ThumbicoOptions.WideAspect))
        {
            flags |= SIIGBF.SIIGBF_WIDETHUMBNAILS;
        }

        if (options.HasFlag(ThumbicoOptions.IconBackground))
        {
            flags |= SIIGBF.SIIGBF_ICONBACKGROUND;
        }

        if (options.HasFlag(ThumbicoOptions.ScaleUp))
        {
            flags |= SIIGBF.SIIGBF_SCALEUP;
        }

        return flags;
    }
}
