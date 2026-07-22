// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;

namespace Thumbico.Tests;

public class AppSettingsTests
{
    /// <summary>
    /// The 2021 build lost this value on every restart without raising an error. This test is the
    /// reason the converter exists; if it ever fails, the colour is silently not being saved.
    /// </summary>
    [Fact]
    public void WhenBackgroundColorIsSavedThenItSurvivesTheRoundTrip()
    {
        SettingsStore store = NewStore();
        Color expected = Color.FromArgb(255, 12, 34, 56);

        store.Save(new AppSettings { BackgroundColor = expected });
        AppSettings loaded = store.Load();

        Assert.Equal(expected.ToArgb(), loaded.BackgroundColor?.ToArgb());
    }

    [Fact]
    public void WhenBackgroundColorHasAlphaThenTheAlphaSurvives()
    {
        SettingsStore store = NewStore();
        Color expected = Color.FromArgb(128, 200, 100, 50);

        store.Save(new AppSettings { BackgroundColor = expected });

        Assert.Equal(expected.ToArgb(), store.Load().BackgroundColor?.ToArgb());
    }

    [Fact]
    public void WhenBackgroundColorIsNullThenItLoadsAsNull()
    {
        SettingsStore store = NewStore();

        store.Save(new AppSettings { BackgroundColor = null });

        Assert.Null(store.Load().BackgroundColor);
    }

    [Fact]
    public void WhenEverythingIsSavedThenEveryValueSurvives()
    {
        SettingsStore store = NewStore();
        AppSettings saved = new()
        {
            WindowBounds = new Rectangle(11, 22, 333, 444),
            WindowMaximized = true,
            SizeSelection = "57 x 69",
            Source = ThumbicoSource.IconOnly,
            Options = ThumbicoOptions.CropToSquare | ThumbicoOptions.ScaleUp,
        };

        store.Save(saved);
        AppSettings loaded = store.Load();

        Assert.Equal(saved.WindowBounds, loaded.WindowBounds);
        Assert.True(loaded.WindowMaximized);
        Assert.Equal(saved.SizeSelection, loaded.SizeSelection);
        Assert.Equal(ThumbicoSource.IconOnly, loaded.Source);
        Assert.Equal(saved.Options, loaded.Options);
    }

    [Fact]
    public void WhenNoFileExistsThenLoadReturnsDefaults()
    {
        AppSettings loaded = NewStore().Load();

        Assert.Null(loaded.BackgroundColor);
        Assert.Equal(ThumbicoSource.Auto, loaded.Source);
        Assert.Equal(ThumbicoOptions.None, loaded.Options);
    }

    [Fact]
    public void WhenTheFileIsCorruptThenLoadReturnsDefaultsInsteadOfThrowing()
    {
        SettingsStore store = NewStore();
        File.WriteAllText(store.FilePath, "{ this is not json");

        AppSettings loaded = store.Load();

        Assert.Equal(ThumbicoSource.Auto, loaded.Source);
    }

    /// <summary>
    /// The installer puts the program in Program Files, where a standard user cannot write, so the
    /// fallback is the normal path for an installed copy rather than an unusual one.
    /// </summary>
    [Fact]
    public void WhenTheDirectoryIsNotWritableThenSavingDoesNotThrow()
    {
        SettingsStore store = new(Path.Combine(Path.GetTempPath(), $"thumbico-missing-{Guid.NewGuid():N}"));

        store.Save(new AppSettings { BackgroundColor = Color.Red });

        Assert.Null(store.Load().BackgroundColor);
    }

    private static SettingsStore NewStore()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"thumbico-settings-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        return new SettingsStore(directory);
    }
}
