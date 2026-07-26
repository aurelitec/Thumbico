// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// The user-facing text of the interface.
/// </summary>
/// <remarks>
/// Plain constants rather than a resx. Localization is not planned, and the resx route cost build
/// machinery that the language server could not see, so every call site showed as an error while the
/// build stayed green. Text that is never displayed does not belong here; a URL that is only ever
/// navigated to lives in <see cref="Urls"/>.
/// </remarks>
internal static class Strings
{
    internal const string AppName = "Thumbico";

    internal const string DropPrompt = "Drop a file, folder, or drive onto this window.";

    internal const string OpenButtonTooltip = "Open a file, folder, or drive";
    internal const string RefreshButtonTooltip = "Ask the shell again";
    internal const string MenuButtonTooltip = "More";
    internal const string PathBoxAccessibleName = "Path of the item to render";
    internal const string SizeBoxAccessibleName = "Requested size";
    internal const string FitToWindow = "Fit to window";

    internal const string StatusAskedFormat = "Asked for {0}";
    internal const string StatusReturnedFormat = "Returned {0}";
    internal const string StatusKindIcon = "Icon";
    internal const string StatusKindThumbnail = "Thumbnail";

    internal const string MenuOpen = "&Open...";
    internal const string MenuMakeBigger = "Make &Bigger";
    internal const string MenuMakeSmaller = "Make &Smaller";
    internal const string MenuRotateFlip = "&Rotate / Flip";
    internal const string MenuRotateLeft = "Rotate &Left";
    internal const string MenuRotateRight = "Rotate &Right";
    internal const string MenuFlipHorizontal = "Flip &Horizontal";
    internal const string MenuFlipVertical = "Flip &Vertical";
    internal const string MenuGrayscale = "&Grayscale";
    internal const string MenuSaveImageAs = "Save Image &As...";
    internal const string MenuCopy = "&Copy";
    internal const string MenuSource = "So&urce";
    internal const string MenuSourceAuto = "Auto";
    internal const string MenuSourceThumbnailOnly = "Thumbnail only";
    internal const string MenuSourceIconOnly = "Icon only";
    internal const string MenuAdvanced = "Ad&vanced";
    internal const string MenuOptionAllowLargerSize = "Allow larger size";
    internal const string MenuOptionCropToSquare = "Crop to square";
    internal const string MenuOptionWideAspect = "Wide aspect";
    internal const string MenuOptionIconBackground = "Icon background";
    internal const string MenuOptionScaleUp = "Scale up";
    internal const string MenuBackground = "Bac&kground";
    internal const string MenuBackgroundCheckerboard = "Checkerboard";
    internal const string MenuBackgroundSolidColor = "Solid Color...";
    internal const string MenuNakedMode = "&Naked Mode";
    internal const string MenuOnlineHelp = "Online &Help";
    internal const string MenuAbout = "&About Thumbico";

    /// <summary>
    /// Written out rather than taken from the Keys enum, which spells these two "Ctrl+Oemplus" and
    /// "Ctrl+OemMinus".
    /// </summary>
    internal const string ShortcutMakeBigger = "Ctrl++";
    internal const string ShortcutMakeSmaller = "Ctrl+-";

    /// <summary>
    /// The save filter's pairs are in the same order as the formats the save handler chooses from.
    /// The two lists are only correct while they agree.
    /// </summary>
    internal const string SaveDialogFilter = "PNG image (*.png)|*.png|BMP image (*.bmp)|*.bmp|"
        + "GIF image (*.gif)|*.gif|JPEG image (*.jpg)|*.jpg|TIFF image (*.tif)|*.tif";

    internal const string OpenDialogFilter = "All files (*.*)|*.*";

    internal const string AboutFormat = "Thumbico {0}\nCopyright (c) 2011-{1} Aurelitec\n"
        + "https://www.aurelitec.com";
}
