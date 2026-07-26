// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// What the interface reads off the current item's path: the name to call it, the name to offer a
/// saved copy, and the format a save should write.
/// </summary>
internal static class ItemPath
{
    /// <summary>
    /// The save formats, in the same order as the pairs in the dialog filter. The two lists are only
    /// correct while they agree, which is what the 2018 build got wrong.
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
    /// Path.GetFileName is documented to return empty when a path ends in a directory or a volume
    /// separator, so a folder path with a trailing slash and a drive root both left the title reading
    /// " - Thumbico". Trailing separators are trimmed first, and anything that still has no name shows
    /// the path itself. That last case is the roots: trimming "C:\" leaves the volume separator, and a
    /// UNC share such as "\\server\share" is a root in its own right. Showing the path keeps whatever
    /// the user typed, so a trailing separator survives into the title there - accepted, since the
    /// alternative loses the backslash that makes "C:\" read as a drive.
    /// </remarks>
    internal static string DisplayName(string path)
    {
        string trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string name = Path.GetFileName(trimmed);

        return string.IsNullOrEmpty(name) ? path : name;
    }

    /// <summary>
    /// The name the save dialog opens with: the item's own, suffixed, so saving never offers to
    /// overwrite the file the thumbico was rendered from.
    /// </summary>
    internal static string DefaultSaveName(string? path)
        => Path.GetFileNameWithoutExtension(path) + "_thumbico";

    /// <summary>
    /// Decides what a save writes, preferring the name the user typed over the filter they left
    /// selected.
    /// </summary>
    /// <param name="fileName">The name the save dialog returned.</param>
    /// <param name="filterIndex">The dialog's one-based filter index, used only when the name carries
    /// no extension this can recognise.</param>
    /// <remarks>
    /// The dialog appends the filter's extension when none was typed, so in the ordinary case the two
    /// agree and the filter decides. They disagree only when the typed name carries an extension of its
    /// own, and then the name has to win: every other program reads a file by its extension, so
    /// honouring the filter instead is what writes BMP bytes into something called .png.
    /// </remarks>
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
