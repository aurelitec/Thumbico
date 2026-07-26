// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing;
using System.Reflection;

namespace Thumbico.Tests;

public class GlyphsTests
{
    private const string FontResource = "Thumbico.Assets.Thumbico.Icons.ttf";

    private const int Size = 32;

    private static readonly HashSet<int> BundledCodePoints = ReadBundledFont();

    public static TheoryData<string, string> DeclaredGlyphs()
    {
        TheoryData<string, string> data = [];

        foreach ((string name, char glyph) in Declared())
        {
            data.Add(name, glyph.ToString());
        }

        return data;
    }

    /// <summary>
    /// Every single-character constant on <see cref="Glyphs"/>, read by reflection so that adding an
    /// icon cannot leave it untested.
    /// </summary>
    private static IEnumerable<(string Name, char Glyph)> Declared()
        => typeof(Glyphs)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(field => field.IsLiteral && field.GetRawConstantValue() is string { Length: 1 })
            .Select(field => (field.Name, ((string)field.GetRawConstantValue()!)[0]));

    /// <summary>
    /// The guard that matters: a code point the subset lacks still renders, because Windows substitutes
    /// another font, so a wrong icon ships looking deliberate.
    /// </summary>
    [Theory]
    [MemberData(nameof(DeclaredGlyphs))]
    public void WhenAGlyphIsDeclaredThenTheBundledFontMapsIt(string name, string glyph)
    {
        Assert.True(
            BundledCodePoints.Contains(glyph[0]),
            $"{name} is U+{(int)glyph[0]:X4}, which the bundled subset does not map. It would render "
            + "as whatever font Windows substitutes.");
    }

    /// <summary>
    /// The subset carries nothing the interface does not use, which is what keeps it small and what
    /// makes regenerating it from the constants correct.
    /// </summary>
    [Fact]
    public void WhenTheFontIsReadThenItMapsExactlyTheDeclaredGlyphs()
    {
        IEnumerable<int> declared = Declared().Select(entry => (int)entry.Glyph).OrderBy(code => code);

        Assert.Equal(declared, BundledCodePoints.OrderBy(code => code));
    }

    [Theory]
    [MemberData(nameof(DeclaredGlyphs))]
    public void WhenAGlyphIsRenderedThenItDrawsSomething(string name, string glyph)
    {
        using Bitmap rendered = Glyphs.Render(glyph, Size, Color.Black);

        Assert.True(HasInk(rendered), $"{name} rendered as a blank bitmap.");
    }

    /// <summary>Two code points can both resolve and still draw the same picture.</summary>
    [Fact]
    public void WhenRotateLeftAndRotateRightAreRenderedThenTheyDiffer()
    {
        using Bitmap left = Glyphs.Render(Glyphs.RotateLeft, Size, Color.Black);
        using Bitmap right = Glyphs.Render(Glyphs.RotateRight, Size, Color.Black);

        Assert.False(AreIdentical(left, right), "Rotate Left and Rotate Right render identically.");
    }

    /// <summary>Dark mode depends on this: an icon baked to a fixed colour vanishes on dark chrome.</summary>
    [Fact]
    public void WhenAColorIsGivenThenTheGlyphIsDrawnInIt()
    {
        using Bitmap red = Glyphs.Render(Glyphs.Open, Size, Color.Red);

        Assert.Contains(Pixels(red), pixel => pixel.A > 128 && pixel.R > 128 && pixel.G < 64);
    }

    /// <summary>Display scale depends on this: the caller asks for the pixel size the DPI needs.</summary>
    [Fact]
    public void WhenASizeIsGivenThenTheBitmapMatchesIt()
    {
        using Bitmap rendered = Glyphs.Render(Glyphs.Open, 48, Color.Black);

        Assert.Equal(new Size(48, 48), rendered.Size);
    }

    private static HashSet<int> ReadBundledFont()
    {
        using Stream stream = typeof(Glyphs).Assembly.GetManifestResourceStream(FontResource)
            ?? throw new InvalidOperationException($"The embedded icon font {FontResource} is missing.");

        byte[] font = new byte[stream.Length];
        stream.ReadExactly(font);

        return FontCodePoints.Read(font);
    }

    private static IEnumerable<Color> Pixels(Bitmap bitmap)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                yield return bitmap.GetPixel(x, y);
            }
        }
    }

    private static bool HasInk(Bitmap bitmap) => Pixels(bitmap).Any(pixel => pixel.A != 0);

    private static bool AreIdentical(Bitmap left, Bitmap right)
        => Pixels(left).SequenceEqual(Pixels(right));
}
