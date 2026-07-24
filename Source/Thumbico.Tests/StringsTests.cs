// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Globalization;

namespace Thumbico.Tests;

/// <summary>
/// Guards the resource generation, which fails by producing nothing rather than by failing the
/// build. Two earlier wirings, the Visual Studio designer generator and GenerateSource, both left
/// the build green and the accessor absent.
/// </summary>
public class StringsTests
{
    [Fact]
    public void WhenAStringIsAskedForThenTheGeneratedAccessorReturnsIt()
    {
        Assert.Equal("Thumbico", Strings.AppName);
    }

    /// <summary>
    /// Pins the resx line-break escaping. A resx has no backslash escapes, so a literal \n in the
    /// value would print as a backslash and an n and collapse this to one line.
    /// </summary>
    [Fact]
    public void WhenTheAboutTextIsFormattedThenItHasThreeLines()
    {
        string about = string.Format(CultureInfo.InvariantCulture, Strings.AboutFormat, "1.0", 2026);

        Assert.Equal(3, about.Split('\n').Length);
        Assert.DoesNotContain(@"\n", about, StringComparison.Ordinal);
    }

    /// <summary>
    /// The dialog filters are pipe separated, and a miscount silently produces a broken dialog
    /// rather than an error.
    /// </summary>
    [Fact]
    public void WhenTheSaveFilterIsReadThenItHasFivePairs()
    {
        Assert.Equal(10, Strings.SaveDialogFilter.Split('|').Length);
    }

    [Fact]
    public void WhenTheOpenFilterIsReadThenItHasOnePair()
    {
        Assert.Equal(2, Strings.OpenDialogFilter.Split('|').Length);
    }
}
