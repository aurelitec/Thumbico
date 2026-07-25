// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing.Drawing2D;
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

    private static readonly PrivateFontCollection Collection = LoadFont();

    /// <summary>
    /// Draws one glyph into a square bitmap over a transparent background, centred on its ink.
    /// </summary>
    /// <param name="glyph">One of the code point constants on this class.</param>
    /// <param name="size">The edge length in pixels, for both the glyph and the bitmap. The caller
    /// derives this from the display scale rather than assuming 16.</param>
    /// <param name="color">The ink colour. The caller takes this from the surface the bitmap will
    /// sit on, so the icon follows the light or dark theme.</param>
    /// <returns>A new bitmap that the caller owns.</returns>
    internal static Bitmap Render(string glyph, int size, Color color)
        => Render(glyph, size, size, color);

    /// <summary>
    /// Draws one glyph centred in a larger square bitmap, the surplus left transparent.
    /// </summary>
    /// <param name="glyph">One of the code point constants on this class.</param>
    /// <param name="size">The edge length the glyph itself is drawn at, in pixels.</param>
    /// <param name="box">The edge length of the bitmap, in pixels. Anything beyond the glyph is
    /// transparent padding, which is how a caller buys space around an icon without enlarging it.</param>
    /// <param name="color">The ink colour. The caller takes this from the surface the bitmap will
    /// sit on, so the icon follows the light or dark theme.</param>
    /// <returns>A new bitmap that the caller owns.</returns>
    /// <remarks>
    /// Drawn as an outline rather than as text, because centring text centres the line box and this
    /// font's line box is taller than its em with no descent to balance it - which left every glyph
    /// sitting high, by a different amount for each. An outline can be asked where its own ink is.
    /// Filling geometry also avoids the question of which text paths can see a memory font.
    /// </remarks>
    internal static Bitmap Render(string glyph, int size, int box, Color color)
    {
        Bitmap bitmap = new(box, box);

        using GraphicsPath path = new();
        using (StringFormat format = StringFormat.GenericTypographic)
        {
            path.AddString(
                glyph, Collection.Families[0], (int)FontStyle.Regular, size, PointF.Empty, format);
        }

        // Centred against the bitmap rather than the glyph, so the padding splits evenly around it.
        RectangleF ink = path.GetBounds();
        using (Matrix centre = new())
        {
            centre.Translate(((box - ink.Width) / 2f) - ink.X, ((box - ink.Height) / 2f) - ink.Y);
            path.Transform(centre);
        }

        using (Graphics graphics = Graphics.FromImage(bitmap))
        using (SolidBrush brush = new(color))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(brush, path);
        }

        return bitmap;
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
