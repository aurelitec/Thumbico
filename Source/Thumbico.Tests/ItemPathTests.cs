// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico.Tests;

public class ItemPathTests
{
    /// <summary>A name is never empty, whatever separators or roots the path ends in.</summary>
    [Theory]
    [InlineData(@"X:\ToDelete\ShareX_thumbico.png", "ShareX_thumbico.png")]
    [InlineData(@"C:\Foo", "Foo")]
    [InlineData(@"C:\Foo\", "Foo")]
    [InlineData(@"C:\Foo\\", "Foo")]
    [InlineData(@"C:\Foo/", "Foo")]
    [InlineData(@"C:\", @"C:\")]
    [InlineData("C:", "C:")]
    [InlineData(@"\\server\share\folder", "folder")]
    [InlineData("shell:MyComputerFolder", "shell:MyComputerFolder")]

    // A UNC share is itself a root, like a drive, so it has no name below it to show.
    [InlineData(@"\\server\share", @"\\server\share")]
    [InlineData(@"\\server\share\", @"\\server\share\")]
    public void WhenAPathIsNamedThenTheNameIsNeverEmpty(string path, string expected)
    {
        Assert.Equal(expected, ItemPath.DisplayName(path));
    }

    [Fact]
    public void WhenASaveNameIsOfferedThenItIsTheItemsOwnNameSuffixed()
    {
        Assert.Equal("icon_thumbico", ItemPath.DefaultSaveName(@"C:\Foo\icon.png"));
    }

    /// <summary>
    /// Taking the format from the filter index alone writes BMP bytes into a file named .png. The first
    /// case is the one that gets it wrong.
    /// </summary>
    [Theory]
    [InlineData("thumb.png", 2, ThumbicoFormat.Png)]
    [InlineData("thumb.bmp", 1, ThumbicoFormat.Bmp)]
    [InlineData("thumb.jpeg", 1, ThumbicoFormat.Jpeg)]
    [InlineData("thumb.TIF", 1, ThumbicoFormat.Tiff)]
    public void WhenTheNameCarriesAnExtensionThenItBeatsTheFilter(
        string fileName, int filterIndex, ThumbicoFormat expected)
    {
        Assert.Equal(expected, ItemPath.FormatFrom(fileName, filterIndex));
    }

    [Theory]
    [InlineData(1, ThumbicoFormat.Png)]
    [InlineData(2, ThumbicoFormat.Bmp)]
    [InlineData(3, ThumbicoFormat.Gif)]
    [InlineData(4, ThumbicoFormat.Jpeg)]
    [InlineData(5, ThumbicoFormat.Tiff)]
    public void WhenTheNameCarriesNoExtensionThenTheFilterDecides(int filterIndex, ThumbicoFormat expected)
    {
        Assert.Equal(expected, ItemPath.FormatFrom("thumb", filterIndex));
    }
}
