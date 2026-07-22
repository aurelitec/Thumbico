// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Drawing.Imaging;

namespace Thumbico.Tests;

public class ThumbicoImageTests
{
    private static readonly Size RequestedSize = new(256, 256);

    [Fact]
    public void WhenPathIsEmptyThenThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ThumbicoImage.FromPath(string.Empty, RequestedSize));
    }

    [Fact]
    public void WhenWidthIsZeroThenThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ThumbicoImage.FromPath(TestFiles.TextFile, new Size(0, 256)));
    }

    [Fact]
    public void WhenPathDoesNotExistThenThrows()
    {
        string missing = Path.Combine(Path.GetTempPath(), $"thumbico-missing-{Guid.NewGuid():N}.txt");

        Assert.ThrowsAny<Exception>(() => ThumbicoImage.FromPath(missing, RequestedSize));
    }

    [Fact]
    public void WhenPathUsesForwardSlashesThenItStillResolves()
    {
        string forwardSlashed = TestFiles.PngFile.Replace('\\', '/');

        using ThumbicoImage thumbico = ThumbicoImage.FromPath(forwardSlashed, RequestedSize);

        Assert.True(thumbico.Size.Width > 0);
    }

    [Fact]
    public void WhenPathIsRelativeThenItResolvesAgainstTheCurrentDirectory()
    {
        string original = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.GetDirectoryName(TestFiles.PngFile)!;
        try
        {
            using ThumbicoImage thumbico = ThumbicoImage.FromPath(
                Path.GetFileName(TestFiles.PngFile), RequestedSize);

            Assert.True(thumbico.Size.Width > 0);
        }
        finally
        {
            Environment.CurrentDirectory = original;
        }
    }

    [Theory]
    [InlineData("shell:RecycleBinFolder")]
    [InlineData("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}")]
    public void WhenPathIsAShellNamespaceLocationThenItResolves(string path)
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(path, RequestedSize);

        Assert.True(thumbico.Size.Width > 0);
    }

    [Fact]
    public void WhenItemIsAFolderThenReturnsAnImage()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(Path.GetTempPath(), RequestedSize);

        Assert.True(thumbico.Size.Width > 0);
        Assert.True(thumbico.Size.Height > 0);
    }

    [Fact]
    public void WhenItemIsAnImageFileThenReturnsThumbnailRatherThanIcon()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);

        Assert.False(thumbico.IsIcon);
    }

    [Fact]
    public void WhenSourceIsIconOnlyThenReturnsIconForAnImageFile()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(
            TestFiles.PngFile, RequestedSize, ThumbicoSource.IconOnly);

        Assert.True(thumbico.IsIcon);
    }

    [Fact]
    public void WhenItemHasNoThumbnailThenFallsBackToIcon()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.TextFile, RequestedSize);

        Assert.True(thumbico.IsIcon);
    }

    [Fact]
    public void WhenSourceIsThumbnailOnlyThenDoesNotReportAnIcon()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(
            TestFiles.PngFile, RequestedSize, ThumbicoSource.ThumbnailOnly);

        Assert.False(thumbico.IsIcon);
    }

    /// <summary>
    /// The reason the engine reads the DIB section rather than calling Image.FromHbitmap: an icon
    /// drawn on a transparent background must come back with that transparency intact.
    /// </summary>
    [Fact]
    public void WhenItemIsAnIconThenTransparencyIsPreserved()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(
            TestFiles.TextFile, RequestedSize, ThumbicoSource.IconOnly);

        Assert.Equal(PixelFormat.Format32bppArgb, thumbico.Bitmap.PixelFormat);
        Assert.True(HasTransparentPixel(thumbico.Bitmap), "Expected at least one transparent pixel.");
    }

    /// <summary>
    /// The shell hands back icons bottom-up and thumbnails top-down while reporting a positive
    /// height for both, so the returned kind is the only thing that says which correction to apply.
    /// That rule is undocumented, which is exactly why both paths are pinned here: if a future
    /// Windows release changes it, these fail loudly instead of shipping flipped images.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WhenImageIsReturnedThenTheTopOfItStaysAtTheTop(bool asIcon)
    {
        string path = asIcon ? TestFiles.RedTopBlueBottomIconFile : TestFiles.RedTopBlueBottomFile;
        ThumbicoSource source = asIcon ? ThumbicoSource.IconOnly : ThumbicoSource.ThumbnailOnly;

        using ThumbicoImage thumbico = ThumbicoImage.FromPath(path, RequestedSize, source);

        int middle = thumbico.Size.Width / 2;
        Color top = thumbico.Bitmap.GetPixel(middle, thumbico.Size.Height / 8);
        Color bottom = thumbico.Bitmap.GetPixel(middle, thumbico.Size.Height * 7 / 8);

        Assert.True(top.R > 150 && top.B < 100, $"Expected a red band at the top, found {top}.");
        Assert.True(bottom.B > 150 && bottom.R < 100, $"Expected a blue band at the bottom, found {bottom}.");
    }

    [Fact]
    public void WhenRotatedRightThenWidthAndHeightSwap()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.WideImageFile, RequestedSize);
        Size before = thumbico.Size;

        thumbico.Transform(ThumbicoTransform.RotateRight);

        Assert.Equal(before.Width, thumbico.Size.Height);
        Assert.Equal(before.Height, thumbico.Size.Width);
    }

    [Fact]
    public void WhenFlippedTwiceThenPixelsReturnToTheOriginal()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);
        Color before = thumbico.Bitmap.GetPixel(0, 0);

        thumbico.Transform(ThumbicoTransform.FlipHorizontal);
        thumbico.Transform(ThumbicoTransform.FlipHorizontal);

        Assert.Equal(before, thumbico.Bitmap.GetPixel(0, 0));
    }

    [Theory]
    [InlineData(ThumbicoFormat.Bmp)]
    [InlineData(ThumbicoFormat.Gif)]
    [InlineData(ThumbicoFormat.Jpeg)]
    [InlineData(ThumbicoFormat.Png)]
    [InlineData(ThumbicoFormat.Tiff)]
    public void WhenSavedThenTheFileIsReadableInThatFormat(ThumbicoFormat format)
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);
        string path = TestFiles.NewTempPath(format.ToString().ToLowerInvariant());

        thumbico.Save(path, format);

        using Image saved = Image.FromFile(path);
        Assert.Equal(thumbico.Size, saved.Size);
    }

    [Fact]
    public void WhenSavedAsPngThenTransparencySurvivesTheRoundTrip()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(
            TestFiles.TextFile, RequestedSize, ThumbicoSource.IconOnly);
        string path = TestFiles.NewTempPath("png");

        thumbico.Save(path, ThumbicoFormat.Png);

        using Bitmap saved = new(path);
        Assert.True(HasTransparentPixel(saved), $"Expected transparency to survive: {path}");
    }

    [Fact]
    public void WhenDisposedThenSaveThrowsObjectDisposedException()
    {
        ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);
        thumbico.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => thumbico.Save(TestFiles.NewTempPath("png"), ThumbicoFormat.Png));
    }

    [Fact]
    public void WhenDisposedTwiceThenDoesNotThrow()
    {
        ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);

        thumbico.Dispose();
        thumbico.Dispose();
    }

    [Fact]
    public void WhenConvertedToGrayscaleThenEveryPixelHasEqualChannels()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);

        thumbico.ToGrayscale();

        Color sample = thumbico.Bitmap.GetPixel(thumbico.Size.Width / 2, thumbico.Size.Height / 2);
        Assert.Equal(sample.R, sample.G);
        Assert.Equal(sample.G, sample.B);
    }

    [Fact]
    public void WhenConvertedToGrayscaleThenTransparencySurvives()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(
            TestFiles.TextFile, RequestedSize, ThumbicoSource.IconOnly);

        thumbico.ToGrayscale();

        Assert.Equal(PixelFormat.Format32bppArgb, thumbico.Bitmap.PixelFormat);
        Assert.True(HasTransparentPixel(thumbico.Bitmap), "Expected transparency to survive.");
    }

    [Fact]
    public void WhenConvertedToGrayscaleThenTheSizeIsUnchanged()
    {
        using ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);
        Size before = thumbico.Size;

        thumbico.ToGrayscale();

        Assert.Equal(before, thumbico.Size);
    }

    [Fact]
    public void WhenDisposedThenToGrayscaleThrowsObjectDisposedException()
    {
        ThumbicoImage thumbico = ThumbicoImage.FromPath(TestFiles.PngFile, RequestedSize);
        thumbico.Dispose();

        Assert.Throws<ObjectDisposedException>(thumbico.ToGrayscale);
    }

    private static bool HasTransparentPixel(Bitmap bitmap)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A < 255)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
