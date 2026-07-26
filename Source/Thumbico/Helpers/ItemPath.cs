// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// What the interface reads off the current item's path: the names to show, and the format a save
/// should write.
/// </summary>
internal static class ItemPath
{
    /// <summary>
    /// The save formats, in the same order as the pairs in the dialog filter. The two are only correct
    /// while they agree.
    /// </summary>
    private static readonly ThumbicoFormat[] SaveFormats =
    [
        ThumbicoFormat.Png,
        ThumbicoFormat.Bmp,
        ThumbicoFormat.Gif,
        ThumbicoFormat.Jpeg,
        ThumbicoFormat.Tiff,
    ];

    /// <summary>
    /// The name to show for an item in the window title.
    /// </summary>
    /// <remarks>
    /// Path.GetFileName returns empty for a path ending in a directory or a volume separator, so a
    /// root has no name to show and falls back to the path itself.
    /// </remarks>
    internal static string DisplayName(string path)
    {
        string trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string name = Path.GetFileName(trimmed);

        return string.IsNullOrEmpty(name) ? path : name;
    }

    /// <summary>
    /// The name the save dialog opens with: the item's own, suffixed so saving cannot offer to
    /// overwrite what the thumbico was rendered from.
    /// </summary>
    internal static string DefaultSaveName(string? path)
        => Path.GetFileNameWithoutExtension(path) + "_thumbico";

    /// <summary>
    /// Decides what a save writes, preferring the extension the user typed over the filter left
    /// selected, because every other program reads a file by its extension.
    /// </summary>
    /// <param name="fileName">The name the save dialog returned.</param>
    /// <param name="filterIndex">One-based, and used only when the name carries no known extension.</param>
    internal static ThumbicoFormat FormatFrom(string fileName, int filterIndex)
        => Path.GetExtension(fileName).ToUpperInvariant() switch
        {
            ".PNG" => ThumbicoFormat.Png,
            ".BMP" => ThumbicoFormat.Bmp,
            ".GIF" => ThumbicoFormat.Gif,
            ".JPG" or ".JPEG" => ThumbicoFormat.Jpeg,
            ".TIF" or ".TIFF" => ThumbicoFormat.Tiff,
            _ => SaveFormats[filterIndex - 1],
        };
}
