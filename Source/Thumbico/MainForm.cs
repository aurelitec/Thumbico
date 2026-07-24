// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Diagnostics;
using System.Globalization;

namespace Thumbico;

/// <summary>
/// The Thumbico window: a toolbar, a canvas, a status bar, and one menu with two entry points.
/// </summary>
internal sealed partial class MainForm : Form
{
    /// <summary>The smallest size worth asking the shell for, used when the canvas is tiny.</summary>
    private const int MinimumRequest = 16;

    private readonly string? _initialPath;

    private ThumbicoImage? _thumbico;
    private string? _path;
    private ThumbicoSource _source = ThumbicoSource.Auto;
    private ThumbicoOptions _options = ThumbicoOptions.None;
    private int _quarterTurns;
    private bool _flipHorizontal;
    private bool _flipVertical;
    private bool _grayscale;

    internal MainForm(string? initialPath)
    {
        this.BuildLayout();
        this._sizeBox.SelectedIndex = 0;
        this._initialPath = initialPath;
    }

    /// <summary>
    /// The requested size. Always the canvas until Task 9 gives the combo a way to fix it.
    /// </summary>
    private Size RequestedSize => new(
        Math.Max(this._canvas.ClientSize.Width, MinimumRequest),
        Math.Max(this._canvas.ClientSize.Height, MinimumRequest));

    /// <summary>
    /// Keeps the window inside the screen it opens on, then renders anything passed on the command
    /// line.
    /// </summary>
    /// <remarks>
    /// The design size is written at 100 percent scale, and font scaling inflates it by more than
    /// the display scale alone: on a 1080p screen at 150 percent it came out 1106 pixels tall and
    /// hung its status bar off the bottom of the desktop. Both the clamp and the first render belong
    /// here rather than in the constructor, because scaling has not been applied while that runs and
    /// Fit to window would size the first request against a canvas that is about to change.
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

        if (!string.IsNullOrWhiteSpace(this._initialPath))
        {
            this.SetPath(this._initialPath);
        }
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
        base.OnDragEnter(e);

        e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
        base.OnDragDrop(e);

        if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } items)
        {
            this.SetPath(items[0]);
        }
    }

    /// <summary>
    /// Re-asks the shell once resizing has finished. Task 9 makes this conditional on the size
    /// still following the window.
    /// </summary>
    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);

        this.Render();
    }

    /// <summary>
    /// F5 reloads, as it did in 1.0 and 1.5. Refresh has no menu item to carry a shortcut, so the
    /// form takes the key itself.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.F5)
        {
            e.SuppressKeyPress = true;
            this.Render();
        }
    }

    /// <summary>
    /// Asks the shell, replays the adjustments, and repaints.
    /// </summary>
    /// <remarks>
    /// Adjustments are replayed rather than accumulated, so changing the size or the source keeps a
    /// rotation the user already applied. The 2018 and 2021 builds discarded it here.
    /// </remarks>
    private void Render()
    {
        if (this._path is null)
        {
            return;
        }

        Size asked = this.RequestedSize;
        ThumbicoImage loaded;

        try
        {
            loaded = ThumbicoImage.FromPath(this._path, asked, this._source, this._options);
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            this.ReportFailure(asked, error);

            return;
        }

        // Read the shell's answer before the adjustments touch it. The status bar reports the shell
        // transaction, and a rotation would otherwise be shown as though the shell had returned the
        // swapped dimensions.
        Size returned = loaded.Size;
        this.ApplyAdjustments(loaded);

        // Hand the canvas the new bitmap before releasing the old one, so it is never holding a
        // reference to something already disposed.
        ThumbicoImage? previous = this._thumbico;
        this._thumbico = loaded;
        this._canvas.Image = loaded.Bitmap;
        previous?.Dispose();

        this._askedLabel.Text = string.Format(
            CultureInfo.CurrentCulture, Strings.StatusAskedFormat, ThumbicoSize.Format(asked));
        this._returnedLabel.Text = string.Format(
            CultureInfo.CurrentCulture, Strings.StatusReturnedFormat, ThumbicoSize.Format(returned));
        this._kindLabel.Text = loaded.IsIcon ? Strings.StatusKindIcon : Strings.StatusKindThumbnail;

        this._saveItem.Enabled = this._copyItem.Enabled = true;
    }

    private void ApplyAdjustments(ThumbicoImage image)
    {
        for (int turn = 0; turn < this._quarterTurns; turn++)
        {
            image.Transform(ThumbicoTransform.RotateRight);
        }

        if (this._flipHorizontal)
        {
            image.Transform(ThumbicoTransform.FlipHorizontal);
        }

        if (this._flipVertical)
        {
            image.Transform(ThumbicoTransform.FlipVertical);
        }

        if (this._grayscale)
        {
            image.ToGrayscale();
        }
    }

    /// <summary>
    /// Reports a failure in the status bar. A modal dialog is the wrong weight for a live preview,
    /// where "this item has no thumbnail" is an ordinary answer.
    /// </summary>
    private void ReportFailure(Size asked, Exception error)
    {
        this._askedLabel.Text = string.Format(
            CultureInfo.CurrentCulture, Strings.StatusAskedFormat, ThumbicoSize.Format(asked));
        this._returnedLabel.Text = error.Message;
        this._kindLabel.Text = string.Empty;
        this._canvas.Image = null;
        this._thumbico?.Dispose();
        this._thumbico = null;
        this._saveItem.Enabled = this._copyItem.Enabled = false;
    }

    private void SetPath(string path)
    {
        this._path = path;
        this._pathBox.Text = path;
        this.Text = $"{Path.GetFileName(path)} - {Strings.AppName}";
        this.Render();
    }

    private void OnOpenClicked(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog = new() { Filter = Strings.OpenDialogFilter };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            this.SetPath(dialog.FileName);
        }
    }

    private void OnRefreshClicked(object? sender, EventArgs e) => this.Render();

    /// <summary>
    /// Drops the menu below the toolbar button. The offset is relative to the strip, which is what
    /// the button's own bounds are measured against.
    /// </summary>
    private void OnMenuButtonClicked(object? sender, EventArgs e)
        => this._menu.Show(
            this._toolStrip,
            new Point(this._menuButton.Bounds.Left, this._menuButton.Bounds.Bottom));

    private void OnPathBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(this._pathBox.Text))
        {
            e.SuppressKeyPress = true;
            this.SetPath(this._pathBox.Text.Trim());
        }
    }

    private void OnSizeBoxCommitted(object? sender, EventArgs e)
    {
    }

    private void OnSizeBoxKeyDown(object? sender, KeyEventArgs e)
    {
    }

    private void OnMakeBigger(object? sender, EventArgs e)
    {
    }

    private void OnMakeSmaller(object? sender, EventArgs e)
    {
    }

    private void OnRotate(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: ThumbicoTransform transform })
        {
            this._quarterTurns = (this._quarterTurns + (transform == ThumbicoTransform.RotateRight ? 1 : 3)) % 4;
            this.Render();
        }
    }

    private void OnFlip(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: ThumbicoTransform transform })
        {
            return;
        }

        if (transform == ThumbicoTransform.FlipHorizontal)
        {
            this._flipHorizontal = !this._flipHorizontal;
        }
        else
        {
            this._flipVertical = !this._flipVertical;
        }

        this.Render();
    }

    private void OnGrayscale(object? sender, EventArgs e)
    {
        this._grayscale = !this._grayscale;
        this._grayscaleItem.Checked = this._grayscale;
        this.Render();
    }

    private void OnSaveAs(object? sender, EventArgs e)
    {
    }

    private void OnCopy(object? sender, EventArgs e)
    {
    }

    private void OnSourceSelected(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: ThumbicoSource source })
        {
            return;
        }

        this._source = source;
        foreach (ToolStripMenuItem item in this._sourceItems)
        {
            item.Checked = (ThumbicoSource)item.Tag! == source;
        }

        this.Render();
    }

    private void OnOptionToggled(object? sender, EventArgs e)
    {
        this._options = ThumbicoOptions.None;
        foreach ((ToolStripMenuItem item, ThumbicoOptions flag) in this._optionItems)
        {
            if (item.Checked)
            {
                this._options |= flag;
            }
        }

        this.Render();
    }

    private void OnCheckerboardSelected(object? sender, EventArgs e)
    {
        this._canvas.SolidBackground = null;
        this._checkerboardItem.Checked = true;
        this._solidColorItem.Checked = false;
    }

    private void OnSolidColorSelected(object? sender, EventArgs e)
    {
        using ColorDialog dialog = new()
        {
            AnyColor = true,
            FullOpen = true,
            Color = this._canvas.SolidBackground ?? Color.White,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        this._canvas.SolidBackground = dialog.Color;
        this._checkerboardItem.Checked = false;
        this._solidColorItem.Checked = true;
    }

    private void OnNakedMode(object? sender, EventArgs e)
    {
    }

    private void OnOnlineHelp(object? sender, EventArgs e)
        => Process.Start(new ProcessStartInfo(Strings.UrlHelp) { UseShellExecute = true });

    private void OnAbout(object? sender, EventArgs e)
        => MessageBox.Show(
            this,
            string.Format(
                CultureInfo.CurrentCulture,
                Strings.AboutFormat,
                Application.ProductVersion,
                DateTime.Now.Year),
            Strings.AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
}
