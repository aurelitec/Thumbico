// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Thumbico;

/// <summary>
/// Shows the rendered thumbico at its true size, scrolling rather than scaling when it does not fit.
/// </summary>
/// <remarks>
/// Scaling for display was rejected deliberately: the product exists to show what the shell produced
/// at a requested size, and a resampled preview is a picture of the resampling instead.
///
/// It also takes no focus, which is equally deliberate. A Panel is neither selectable nor a tab stop,
/// and leaving it that way makes the toolbar the only thing the Tab key ever visits. The cost is that
/// scrolling is by wheel and scrollbar rather than by arrow key.
/// </remarks>
internal sealed class ThumbicoCanvas : Panel
{
    /// <summary>The checkerboard square size at 100 percent display scale.</summary>
    private const int TileSize = 24;

    private Bitmap? _image;
    private Color? _solidBackground;
    private TextureBrush? _checkerboard;

    internal ThumbicoCanvas()
    {
        this.AutoScroll = true;
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.ResizeRedraw, true);
    }

    /// <summary>Gets or sets the image to show. The canvas does not own it.</summary>
    /// <remarks>
    /// Hidden from designer serialization, as every property below is. Both hold live state that is
    /// rebuilt on each render, so there is nothing a designer could meaningfully persist. Analyzer
    /// WFO1000 requires the intent to be stated rather than left to the default.
    /// </remarks>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal Bitmap? Image
    {
        get => this._image;
        set
        {
            this._image = value;
            this.AutoScrollMinSize = value?.Size ?? Size.Empty;
            this.Invalidate();
        }
    }

    /// <summary>Gets or sets a flat backdrop colour, or null for the checkerboard.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal Color? SolidBackground
    {
        get => this._solidBackground;
        set
        {
            this._solidBackground = value;
            this.Invalidate();
        }
    }

    /// <summary>
    /// Rebuilds the checkerboard when the system switches between light and dark. Only SystemColors
    /// values follow the theme on their own, and none of this backdrop uses them.
    /// </summary>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);

        this.DiscardCheckerboard();
    }

    /// <summary>
    /// Rebuilds the checkerboard when the display scale changes, since its squares are sized in
    /// device pixels.
    /// </summary>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);

        this.DiscardCheckerboard();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this.PaintBackdrop(e.Graphics);

        if (this._image is null)
        {
            TextRenderer.DrawText(
                e.Graphics,
                Strings.DropPrompt,
                this.Font,
                this.ClientRectangle,
                SystemColors.GrayText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            return;
        }

        // Centred while it fits, pinned to the origin once it does not, so scrolling reaches every edge.
        int x = Math.Max(0, (this.ClientSize.Width - this._image.Width) / 2) + this.AutoScrollPosition.X;
        int y = Math.Max(0, (this.ClientSize.Height - this._image.Height) / 2) + this.AutoScrollPosition.Y;

        e.Graphics.DrawImageUnscaled(this._image, x, y);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._checkerboard?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void DiscardCheckerboard()
    {
        this._checkerboard?.Dispose();
        this._checkerboard = null;
        this.Invalidate();
    }

    private void PaintBackdrop(Graphics graphics)
    {
        if (this._solidBackground is Color solid)
        {
            using SolidBrush brush = new(solid);
            graphics.FillRectangle(brush, this.ClientRectangle);

            return;
        }

        this._checkerboard ??= this.CreateCheckerboard();
        graphics.FillRectangle(this._checkerboard, this.ClientRectangle);
    }

    /// <summary>
    /// Builds the pattern that makes transparent areas of an icon visible, in the current theme and
    /// at the current display scale.
    /// </summary>
    private TextureBrush CreateCheckerboard()
    {
        bool dark = Application.IsDarkModeEnabled;
        Color light = dark ? Color.FromArgb(56, 56, 56) : Color.FromArgb(250, 250, 250);
        Color shade = dark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(226, 226, 226);

        // Derive the tile from its half so it cannot come out odd, which would leave a seam where
        // the two shaded quarters fail to meet the tile edge.
        int half = TileSize * this.DeviceDpi / 96 / 2;
        int tile = half * 2;

        Bitmap pattern = new(tile, tile);
        using (Graphics graphics = Graphics.FromImage(pattern))
        using (SolidBrush brush = new(shade))
        {
            graphics.Clear(light);
            graphics.FillRectangle(brush, 0, 0, half, half);
            graphics.FillRectangle(brush, half, half, half, half);
        }

        return new TextureBrush(pattern) { WrapMode = WrapMode.Tile };
    }
}
