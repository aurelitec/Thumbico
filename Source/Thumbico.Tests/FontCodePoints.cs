// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Buffers.Binary;

namespace Thumbico.Tests;

/// <summary>
/// Reads the set of characters a TrueType font actually maps.
/// </summary>
/// <remarks>
/// Rendering cannot answer this question. A code point the font lacks is not reported: GDI+ either
/// draws nothing or silently substitutes another font's glyph, depending only on whether Windows
/// happens to ship a font covering that range. Both outcomes look like success. Reading the
/// character map is the only way to distinguish a glyph the bundled font supplies from one Windows
/// supplied on its behalf, and that distinction is the whole point of bundling the font.
///
/// Only cmap format 4 is handled, which is what the committed subset uses and what fontTools emits
/// for a Basic Multilingual Plane font. A font using any other format throws rather than silently
/// reporting an empty set.
/// </remarks>
internal static class FontCodePoints
{
    private const int MicrosoftPlatform = 3;
    private const int UnicodeBmpEncoding = 1;

    internal static HashSet<int> Read(ReadOnlySpan<byte> font)
    {
        int tables = BinaryPrimitives.ReadUInt16BigEndian(font[4..]);
        int cmap = FindTable(font, tables, "cmap")
            ?? throw new InvalidDataException("The font has no cmap table.");

        int subtables = BinaryPrimitives.ReadUInt16BigEndian(font[(cmap + 2)..]);
        for (int i = 0; i < subtables; i++)
        {
            int record = cmap + 4 + (i * 8);
            int platform = BinaryPrimitives.ReadUInt16BigEndian(font[record..]);
            int encoding = BinaryPrimitives.ReadUInt16BigEndian(font[(record + 2)..]);

            if (platform == MicrosoftPlatform && encoding == UnicodeBmpEncoding)
            {
                int offset = (int)BinaryPrimitives.ReadUInt32BigEndian(font[(record + 4)..]);

                return ReadFormat4(font[(cmap + offset)..]);
            }
        }

        throw new InvalidDataException("The font has no Microsoft Unicode BMP cmap subtable.");
    }

    private static int? FindTable(ReadOnlySpan<byte> font, int tables, string tag)
    {
        for (int i = 0; i < tables; i++)
        {
            int record = 12 + (i * 16);

            if (font[record] == tag[0]
                && font[record + 1] == tag[1]
                && font[record + 2] == tag[2]
                && font[record + 3] == tag[3])
            {
                return (int)BinaryPrimitives.ReadUInt32BigEndian(font[(record + 8)..]);
            }
        }

        return null;
    }

    private static HashSet<int> ReadFormat4(ReadOnlySpan<byte> table)
    {
        int format = BinaryPrimitives.ReadUInt16BigEndian(table);
        if (format != 4)
        {
            throw new InvalidDataException($"Expected a format 4 cmap subtable, found format {format}.");
        }

        int segments = BinaryPrimitives.ReadUInt16BigEndian(table[6..]) / 2;
        int endCodes = 14;
        int startCodes = endCodes + (segments * 2) + 2;
        int deltas = startCodes + (segments * 2);
        int rangeOffsets = deltas + (segments * 2);

        HashSet<int> mapped = [];
        for (int segment = 0; segment < segments; segment++)
        {
            int end = BinaryPrimitives.ReadUInt16BigEndian(table[(endCodes + (segment * 2))..]);
            int start = BinaryPrimitives.ReadUInt16BigEndian(table[(startCodes + (segment * 2))..]);
            int delta = BinaryPrimitives.ReadInt16BigEndian(table[(deltas + (segment * 2))..]);
            int rangeOffsetAt = rangeOffsets + (segment * 2);
            int rangeOffset = BinaryPrimitives.ReadUInt16BigEndian(table[rangeOffsetAt..]);

            for (int code = start; code <= end && code != 0xFFFF; code++)
            {
                int glyph = rangeOffset == 0
                    ? (code + delta) & 0xFFFF
                    : ReadGlyphFromArray(table, rangeOffsetAt, rangeOffset, code, start, delta);

                // Glyph 0 is .notdef, which means the segment covers the code point without
                // supplying a shape for it.
                if (glyph != 0)
                {
                    mapped.Add(code);
                }
            }
        }

        return mapped;
    }

    private static int ReadGlyphFromArray(
        ReadOnlySpan<byte> table, int rangeOffsetAt, int rangeOffset, int code, int start, int delta)
    {
        int at = rangeOffsetAt + rangeOffset + ((code - start) * 2);
        int glyph = BinaryPrimitives.ReadUInt16BigEndian(table[at..]);

        return glyph == 0 ? 0 : (glyph + delta) & 0xFFFF;
    }
}
