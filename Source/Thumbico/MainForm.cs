// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// The Thumbico window: a toolbar, a canvas, a status bar, and one menu with two entry points.
/// </summary>
internal sealed partial class MainForm : Form
{
    internal MainForm(string? initialPath)
    {
        this.BuildLayout();

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            this._pathBox.Text = initialPath;
        }
    }

    /// <summary>
    /// Keeps the window inside the screen it opens on.
    /// </summary>
    /// <remarks>
    /// The design size is written at 100 percent scale, and font scaling inflates it by more than
    /// the display scale alone: on a 1080p screen at 150 percent it came out 1106 pixels tall and
    /// hung its status bar off the bottom of the desktop. Clamping is done here rather than in
    /// BuildLayout because scaling has not been applied yet while the constructor runs.
    /// </remarks>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Rectangle working = Screen.FromControl(this).WorkingArea;
        this.Size = new Size(
            Math.Min(this.Width, working.Width),
            Math.Min(this.Height, working.Height));
        this.Location = new Point(
            Math.Clamp(this.Left, working.Left, working.Right - this.Width),
            Math.Clamp(this.Top, working.Top, working.Bottom - this.Height));
    }

    private void OnOpenClicked(object? sender, EventArgs e)
    {
    }

    private void OnRefreshClicked(object? sender, EventArgs e)
    {
    }

    private void OnMenuButtonClicked(object? sender, EventArgs e)
    {
    }

    private void OnCanvasMouseUp(object? sender, MouseEventArgs e)
    {
    }

    private void OnPathBoxKeyDown(object? sender, KeyEventArgs e)
    {
    }

    private void OnSizeBoxCommitted(object? sender, EventArgs e)
    {
    }

    private void OnSizeBoxKeyDown(object? sender, KeyEventArgs e)
    {
    }
}
