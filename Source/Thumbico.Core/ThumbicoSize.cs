// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Globalization;

namespace Thumbico;

/// <summary>
/// Converts between a size and the text a user types for it.
/// </summary>
/// <remarks>
/// Shared by every frontend so that a size typed into the window and a size passed on a command line
/// are read the same way.
/// </remarks>
public static class ThumbicoSize
{
    /// <summary>
    /// The largest dimension accepted, which is the limit the 2018 size dialog enforced. That was a
    /// spinner and could not be overtyped; free text can, so the same limit now needs checking.
    /// </summary>
    public const int MaximumDimension = 10000;

    /// <summary>
    /// Parses a size written as a single number, meaning a square, or as two numbers separated by
    /// the letter x or a multiplication sign.
    /// </summary>
    /// <param name="text">The text to read. Surrounding and inner whitespace is ignored.</param>
    /// <param name="size">Receives the parsed size, or <see cref="Size.Empty"/> on failure.</param>
    /// <returns>True when every dimension parsed and fell between 1 and
    /// <see cref="MaximumDimension"/>.</returns>
    public static bool TryParse(string? text, out Size size)
    {
        size = Size.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        ReadOnlySpan<char> span = text.AsSpan().Trim();
        int separator = span.IndexOfAny('x', 'X', '×');

        if (separator < 0)
        {
            if (!TryParseDimension(span, out int side))
            {
                return false;
            }

            size = new Size(side, side);
            return true;
        }

        if (!TryParseDimension(span[..separator], out int width)
            || !TryParseDimension(span[(separator + 1)..], out int height))
        {
            return false;
        }

        size = new Size(width, height);
        return true;
    }

    /// <summary>
    /// Writes a size in the form <see cref="TryParse"/> reads back.
    /// </summary>
    public static string Format(Size size)
        => string.Create(CultureInfo.InvariantCulture, $"{size.Width} x {size.Height}");

    /// <summary>
    /// Reads one dimension. NumberStyles.None is what rejects signs, decimal points, and group
    /// separators, so only bare digits survive.
    /// </summary>
    private static bool TryParseDimension(ReadOnlySpan<char> text, out int value)
        => int.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out value)
            && value >= 1
            && value <= MaximumDimension;
}
