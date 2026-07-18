// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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

        // Auto asks for a real thumbnail first so callers get the richest image available, and
        // falls back to the icon for the many items that have no thumbnail at all.
        if (source == ThumbicoSource.Auto)
        {
            if (TryGetBitmap(factory, requested, flags | SIIGBF.SIIGBF_THUMBNAILONLY, out Bitmap? thumbnail))
            {
                isIcon = false;
                return thumbnail;
            }

            isIcon = true;
            return GetBitmap(factory, requested, flags | SIIGBF.SIIGBF_ICONONLY);
        }

        isIcon = source == ThumbicoSource.IconOnly;
        SIIGBF sourceFlag = source == ThumbicoSource.IconOnly
            ? SIIGBF.SIIGBF_ICONONLY
            : SIIGBF.SIIGBF_THUMBNAILONLY;

        return GetBitmap(factory, requested, flags | sourceFlag);
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
    private static unsafe Bitmap ToManagedBitmap(DeleteObjectSafeHandle hbitmap)
    {
        DIBSECTION dib = default;
        Span<byte> buffer = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref dib, 1));
        int copied = PInvoke.GetObject(hbitmap, buffer);

        // Anything that is not a 32-bit DIB section has no alpha channel worth preserving, so the
        // plain GDI+ conversion is already correct for it.
        if (copied < buffer.Length || dib.dsBmih.biBitCount != 32)
        {
            return Image.FromHbitmap(hbitmap.DangerousGetHandle());
        }

        int width = dib.dsBm.bmWidth;
        int height = Math.Abs(dib.dsBmih.biHeight);
        int sourceStride = dib.dsBm.bmWidthBytes;
        byte* sourceRow = (byte*)dib.dsBm.bmBits;

        // A positive height means the rows are stored bottom-up. Walking them in reverse is what
        // keeps icons upright, without a corrective flip afterwards.
        if (dib.dsBmih.biHeight > 0)
        {
            sourceRow += (long)(height - 1) * sourceStride;
            sourceStride = -sourceStride;
        }

        // The pixels must be copied rather than wrapped: the caller frees the native bitmap as soon
        // as this returns, and GDI+ hands back a view rather than a copy when the format matches.
        Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
        BitmapData target = bitmap.LockBits(
            new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            long rowBytes = (long)width * 4;
            byte* targetRow = (byte*)target.Scan0;

            for (int y = 0; y < height; y++)
            {
                Buffer.MemoryCopy(sourceRow, targetRow, rowBytes, rowBytes);
                sourceRow += sourceStride;
                targetRow += target.Stride;
            }
        }
        finally
        {
            bitmap.UnlockBits(target);
        }

        return bitmap;
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
