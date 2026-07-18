// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Drawing.Imaging;

namespace Thumbico;

/// <summary>
/// The thumbnail or icon of a file, folder, drive, or any other shell item.
/// </summary>
public sealed class ThumbicoImage : IDisposable
{
    private ThumbicoImage(Bitmap bitmap, bool isIcon)
    {
        this.Bitmap = bitmap;
        this.IsIcon = isIcon;
    }

    /// <summary>
    /// Gets the rendered image. The instance owns it; do not dispose it separately.
    /// </summary>
    public Bitmap Bitmap { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the shell returned an icon rather than a thumbnail.
    /// </summary>
    public bool IsIcon { get; }

    /// <summary>
    /// Gets the size the shell actually produced, which is often smaller than the size requested.
    /// </summary>
    public Size Size => this.Bitmap.Size;

    /// <summary>
    /// Renders the shell item at the given path.
    /// </summary>
    /// <param name="path">Path of any shell item: a file, folder, drive, or namespace location.</param>
    /// <param name="size">The size to request. The shell treats this as an upper bound unless
    /// <see cref="ThumbicoOptions.ScaleUp"/> or <see cref="ThumbicoOptions.AllowLargerSize"/> is set.</param>
    /// <param name="source">Whether to prefer a thumbnail, an icon, or the best available.</param>
    /// <param name="options">Options that shape how the shell renders the image.</param>
    /// <returns>The rendered thumbnail or icon.</returns>
    public static ThumbicoImage FromPath(
        string path,
        Size size,
        ThumbicoSource source = ThumbicoSource.Auto,
        ThumbicoOptions options = ThumbicoOptions.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size.Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size.Height);

        Bitmap bitmap = ShellImage.Get(NormalizePath(path), size, source, options, out bool isIcon);

        return new ThumbicoImage(bitmap, isIcon);
    }

    /// <summary>
    /// Saves the image to a file, overwriting it if it already exists.
    /// </summary>
    /// <param name="path">The file to write.</param>
    /// <param name="format">The image file format to write.</param>
    public void Save(string path, ThumbicoFormat format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ObjectDisposedException.ThrowIf(this.disposed, this);

        this.Bitmap.Save(path, ToImageFormat(format));
    }

    /// <summary>
    /// Rotates or flips the image in place.
    /// </summary>
    /// <param name="transform">The rotation or flip to apply.</param>
    public void Transform(ThumbicoTransform transform)
    {
        ObjectDisposedException.ThrowIf(this.disposed, this);

        this.Bitmap.RotateFlip(ToRotateFlipType(transform));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.Bitmap.Dispose();
        this.disposed = true;
    }

    /// <summary>
    /// Resolves a filesystem path to the fully qualified form the shell parser requires, which
    /// relative paths and forward slashes are not. Shell namespace strings are passed through.
    /// </summary>
    private static string NormalizePath(string path)
        => path.StartsWith("shell:", StringComparison.OrdinalIgnoreCase) || path.StartsWith("::")
            ? path
            : Path.GetFullPath(path);

    private static ImageFormat ToImageFormat(ThumbicoFormat format) => format switch
    {
        ThumbicoFormat.Bmp => ImageFormat.Bmp,
        ThumbicoFormat.Gif => ImageFormat.Gif,
        ThumbicoFormat.Jpeg => ImageFormat.Jpeg,
        ThumbicoFormat.Png => ImageFormat.Png,
        ThumbicoFormat.Tiff => ImageFormat.Tiff,
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };

    private static RotateFlipType ToRotateFlipType(ThumbicoTransform transform) => transform switch
    {
        ThumbicoTransform.RotateLeft => RotateFlipType.Rotate270FlipNone,
        ThumbicoTransform.RotateRight => RotateFlipType.Rotate90FlipNone,
        ThumbicoTransform.FlipHorizontal => RotateFlipType.RotateNoneFlipX,
        ThumbicoTransform.FlipVertical => RotateFlipType.RotateNoneFlipY,
        _ => throw new ArgumentOutOfRangeException(nameof(transform)),
    };

    private bool disposed;
}
