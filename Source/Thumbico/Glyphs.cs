// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Thumbico;

/// <summary>
/// The bundled icon font and the code points the interface draws from it.
/// </summary>
/// <remarks>
/// A subset of Microsoft's Fluent UI System Icons, embedded rather than looked up by family name so
/// that there is no font for a machine to be missing. Each constant records the upstream icon name,
/// which is what regenerating the subset needs; see THIRD-PARTY-NOTICES.md.
/// </remarks>
internal static class Glyphs
{
    internal const string Open = "\uE90B";           // folder_open
    internal const string Refresh = "\uE0BF";        // arrow_clockwise
    internal const string More = "\uEC72";           // more_horizontal
    internal const string SaveAs = "\uEFA1";         // save_image
    internal const string Copy = "\uE5D7";           // copy
    internal const string RotateFlip = "\uE143";     // arrow_rotate_clockwise
    internal const string RotateLeft = "\uEF85";     // rotate_left
    internal const string RotateRight = "\uEF87";    // rotate_right
    internal const string FlipHorizontal = "\uE8D9"; // flip_horizontal
    internal const string FlipVertical = "\uE8DB";   // flip_vertical
    internal const string Grayscale = "\uE49A";      // circle_half_fill
    internal const string NakedMode = "\uE951";      // full_screen_maximize
    internal const string ZoomIn = "\uF60F";         // zoom_in
    internal const string ZoomOut = "\uF611";        // zoom_out
    internal const string Source = "\uEA52";         // image
    internal const string Background = "\uE566";     // color
    internal const string Help = "\uEF07";           // question_circle
    internal const string About = "\uEA88";          // info

    private const string ResourceName = "Thumbico.Assets.Thumbico.Icons.ttf";

    /// <summary>
    /// Alpha below which a pixel counts as empty when the ink is measured, so that the faintest
    /// antialiasing does not widen the result by a pixel on every edge.
    /// </summary>
    private const int InkAlphaThreshold = 24;

    private static readonly PrivateFontCollection Collection = LoadFont();

    /// <summary>
    /// Draws one glyph into a square bitmap over a transparent background, centred on its ink.
    /// </summary>
    /// <param name="glyph">One of the code point constants on this class.</param>
    /// <param name="size">The edge length in pixels. The caller derives this from the display scale
    /// rather than assuming 16.</param>
    /// <param name="color">The ink colour. The caller takes this from the surface the bitmap will
    /// sit on, so the icon follows the light or dark theme.</param>
    /// <returns>A new bitmap that the caller owns.</returns>
    /// <remarks>
    /// Drawn twice on purpose. GDI+ centres the text line box rather than the visible glyph, and this
    /// font's line box is taller than its em while having no descent to balance it, so one pass leaves
    /// every glyph sitting high in the bitmap - by a different amount for each, which is why no fixed
    /// correction would do. The first pass finds where the ink landed and the second redraws it
    /// centred on that.
    /// </remarks>
    internal static Bitmap Render(string glyph, int size, Color color)
    {
        using Bitmap probe = Draw(glyph, size, Color.Black, PointF.Empty);
        Rectangle ink = InkBounds(probe);

        PointF offset = ink.IsEmpty
            ? PointF.Empty
            : new PointF(((size - ink.Width) / 2f) - ink.X, ((size - ink.Height) / 2f) - ink.Y);

        return Draw(glyph, size, color, offset);
    }

    private static Bitmap Draw(string glyph, int size, Color color, PointF offset)
    {
        Bitmap bitmap = new(size, size);

        using (Graphics graphics = Graphics.FromImage(bitmap))
        using (Font font = new(Collection.Families[0], size, GraphicsUnit.Pixel))
        using (SolidBrush brush = new(color))
        using (StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,

            // The layout rectangle is deliberately moved off the bitmap, and clipping it to the
            // rectangle rather than to the bitmap would shave the shifted edge.
            FormatFlags = StringFormatFlags.NoClip,
        })
        {
            // DrawString is the GDI+ path, which is the one documented to see a memory font.
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(glyph, font, brush, new RectangleF(offset.X, offset.Y, size, size), format);
        }

        return bitmap;
    }

    /// <summary>
    /// The smallest rectangle holding every pixel the glyph actually marked.
    /// </summary>
    private static Rectangle InkBounds(Bitmap bitmap)
    {
        int left = int.MaxValue;
        int top = int.MaxValue;
        int right = -1;
        int bottom = -1;

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A > InkAlphaThreshold)
                {
                    left = Math.Min(left, x);
                    right = Math.Max(right, x);
                    top = Math.Min(top, y);
                    bottom = Math.Max(bottom, y);
                }
            }
        }

        return right < 0 ? Rectangle.Empty : new Rectangle(left, top, right - left + 1, bottom - top + 1);
    }

    private static PrivateFontCollection LoadFont()
    {
        using Stream stream = typeof(Glyphs).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"The embedded icon font {ResourceName} is missing.");

        byte[] font = new byte[stream.Length];
        stream.ReadExactly(font);

        // Whether GDI+ copies this buffer or keeps reading it is not documented either way, so it is
        // allocated once and never freed. The cost of the precaution is four kilobytes for the
        // lifetime of the process; the cost of being wrong is a font that decodes to garbage.
        IntPtr buffer = Marshal.AllocCoTaskMem(font.Length);
        Marshal.Copy(font, 0, buffer, font.Length);

        PrivateFontCollection collection = new();
        collection.AddMemoryFont(buffer, font.Length);

        return collection;
    }
}
