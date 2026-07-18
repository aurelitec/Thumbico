// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Drawing.Drawing2D;

namespace Thumbico;

/// <summary>
/// A bare window that renders the dropped item as large as it will fit.
/// </summary>
/// <remarks>
/// This is the walking skeleton for the real interface: no menus, no toolbar, no settings. It
/// exists to exercise the engine in a running application and to make the result visible.
/// </remarks>
internal sealed class MainForm : Form
{
    private const int MinimumRequest = 16;

    private static readonly TextureBrush Checkerboard = CreateCheckerboard();

    private ThumbicoImage? thumbico;
    private string? path;

    internal MainForm(string? initialPath)
    {
        this.Text = "Thumbico - drop a file, folder, or drive here";
        this.ClientSize = new Size(720, 720);
        this.MinimumSize = new Size(240, 240);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.AllowDrop = true;
        this.DoubleBuffered = true;

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            this.Show(initialPath);
        }
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
        e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } items)
        {
            this.Show(items[0]);
        }
    }

    /// <summary>
    /// Re-requests the image once the user has finished resizing, so the shell renders it at the
    /// new size rather than the window scaling a smaller bitmap up.
    /// </summary>
    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);

        if (this.path is not null)
        {
            this.Show(this.path);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.FillRectangle(Checkerboard, this.ClientRectangle);

        if (this.thumbico is null)
        {
            TextRenderer.DrawText(
                e.Graphics,
                "Drop a file, folder, or drive onto this window.",
                this.Font,
                this.ClientRectangle,
                SystemColors.GrayText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            return;
        }

        Bitmap bitmap = this.thumbico.Bitmap;
        e.Graphics.DrawImageUnscaled(
            bitmap,
            (this.ClientSize.Width - bitmap.Width) / 2,
            (this.ClientSize.Height - bitmap.Height) / 2);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.thumbico?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Renders the checker pattern that makes transparent areas of an icon visible.
    /// </summary>
    private static TextureBrush CreateCheckerboard()
    {
        Bitmap tile = new(24, 24);
        using (Graphics graphics = Graphics.FromImage(tile))
        {
            graphics.Clear(Color.FromArgb(255, 250, 250, 250));
            using SolidBrush shade = new(Color.FromArgb(255, 226, 226, 226));
            graphics.FillRectangle(shade, 0, 0, 12, 12);
            graphics.FillRectangle(shade, 12, 12, 12, 12);
        }

        return new TextureBrush(tile) { WrapMode = WrapMode.Tile };
    }

    private void Show(string itemPath)
    {
        ThumbicoImage? loaded = null;
        try
        {
            loaded = ThumbicoImage.FromPath(
                itemPath,
                new Size(
                    Math.Max(this.ClientSize.Width, MinimumRequest),
                    Math.Max(this.ClientSize.Height, MinimumRequest)));
        }
        catch (Exception error)
        {
            this.Text = $"Thumbico - {Path.GetFileName(itemPath)} - {error.Message}";
        }

        if (loaded is null)
        {
            return;
        }

        this.thumbico?.Dispose();
        this.thumbico = loaded;
        this.path = itemPath;

        this.Text = $"Thumbico - {Path.GetFileName(itemPath)} - "
            + $"{loaded.Size.Width} x {loaded.Size.Height} {(loaded.IsIcon ? "icon" : "thumbnail")}";

        this.Invalidate();
    }
}
