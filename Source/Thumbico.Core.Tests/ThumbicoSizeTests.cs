// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;

namespace Thumbico.Tests;

public class ThumbicoSizeTests
{
    [Theory]
    [InlineData("256", 256, 256)]
    [InlineData("  256  ", 256, 256)]
    [InlineData("57x69", 57, 69)]
    [InlineData("57X69", 57, 69)]
    [InlineData("57×69", 57, 69)]
    [InlineData("57 x 69", 57, 69)]
    [InlineData("1", 1, 1)]
    [InlineData("10000", 10000, 10000)]
    public void WhenTextIsValidThenItParses(string text, int width, int height)
    {
        Assert.True(ThumbicoSize.TryParse(text, out Size size));
        Assert.Equal(new Size(width, height), size);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-5")]
    [InlineData("57x")]
    [InlineData("x69")]
    [InlineData("57x0")]
    [InlineData("57x69x12")]
    [InlineData("10001")]
    [InlineData("99999999999999999999")]
    [InlineData("1.5")]
    [InlineData("1,000")]
    public void WhenTextIsInvalidThenItDoesNotParse(string? text)
    {
        Assert.False(ThumbicoSize.TryParse(text, out Size size));
        Assert.Equal(Size.Empty, size);
    }

    [Fact]
    public void WhenSizeIsFormattedThenItParsesBackToTheSameSize()
    {
        Size original = new(57, 69);

        Assert.True(ThumbicoSize.TryParse(ThumbicoSize.Format(original), out Size parsed));
        Assert.Equal(original, parsed);
    }

    [Fact]
    public void WhenSizeIsFormattedThenItReadsAsTwoNumbers()
    {
        Assert.Equal("512 x 512", ThumbicoSize.Format(new Size(512, 512)));
    }
}
