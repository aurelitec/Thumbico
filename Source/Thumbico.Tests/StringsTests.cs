// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Globalization;

namespace Thumbico.Tests;

/// <summary>
/// Covers the few strings whose internal structure the code depends on. The rest are plain constants
/// and the compiler already checks every use of them.
/// </summary>
public class StringsTests
{
    /// <summary>
    /// The About box is laid out by its line breaks, so writing this as a verbatim string would
    /// collapse it to one line and print the escapes instead.
    /// </summary>
    [Fact]
    public void WhenTheAboutTextIsFormattedThenItHasThreeLines()
    {
        string about = string.Format(CultureInfo.InvariantCulture, Strings.AboutFormat, "1.0", 2026);

        Assert.Equal(3, about.Split('\n').Length);
    }

    /// <summary>
    /// The filters are pipe separated, and a miscount silently produces a broken dialog rather than
    /// an error. The save filter's five pairs also have to stay in step with the format list that the
    /// save handler indexes by filter position.
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
