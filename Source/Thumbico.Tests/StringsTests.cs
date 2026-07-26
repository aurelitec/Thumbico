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
    /// <summary>The About box is laid out by its line breaks, which a verbatim string would print.</summary>
    [Fact]
    public void WhenTheAboutTextIsFormattedThenItHasThreeLines()
    {
        string about = string.Format(CultureInfo.InvariantCulture, Strings.AboutFormat, "1.0", 2026);

        Assert.Equal(3, about.Split('\n').Length);
    }

    /// <summary>
    /// A miscounted pipe gives a broken dialog rather than an error, and the five pairs must stay in
    /// step with the format list the save handler indexes by position.
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
