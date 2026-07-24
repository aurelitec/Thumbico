// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;

namespace Thumbico;

/// <summary>
/// The Thumbico window: a toolbar, a canvas, a status bar, and one menu with two entry points.
/// </summary>
internal sealed partial class MainForm : Form
{
    /// <summary>The smallest size worth asking the shell for, used when the canvas is tiny.</summary>
    private const int MinimumRequest = 16;

    /// <summary>
    /// What Make Bigger multiplies the request by, and Make Smaller divides it by. A factor rather
    /// than the flat 20 pixels of 1.0 and 1.5, because those versions overwrote the size box with
    /// the size the shell returned; here the box holds the request, so a fixed step applied to a
    /// number the shell is already ignoring would often produce no visible change at all.
    /// </summary>
    private const double SizeStep = 1.25;

    /// <summary>
    /// The save formats, in the same order as the pairs in the dialog filter. The two lists are
    /// only correct while they agree, which is what the 2018 build got wrong.
    /// </summary>
    private static readonly ThumbicoFormat[] SaveFormats =
    [
        ThumbicoFormat.Png,
        ThumbicoFormat.Bmp,
        ThumbicoFormat.Gif,
        ThumbicoFormat.Jpeg,
        ThumbicoFormat.Tiff,
    ];

    private readonly string? _initialPath;

    private ThumbicoImage? _thumbico;
    private string? _path;
    private Size? _fixedSize;
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
    /// The requested size: whatever the user chose, or the canvas itself in Fit to window mode.
    /// </summary>
    private Size RequestedSize => this._fixedSize ?? new Size(
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
    /// Re-asks the shell once resizing has finished, but only when the size follows the window.
    /// </summary>
    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);

        if (this._fixedSize is null)
        {
            this.Render();
        }
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

    private void OnSizeBoxCommitted(object? sender, EventArgs e) => this.CommitSizeText();

    private void OnSizeBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            this.CommitSizeText();
        }
    }

    /// <summary>
    /// Reads the combo. Unparseable text falls back to the last good value rather than raising an
    /// error, because this is a live control and not a submitted form.
    /// </summary>
    private void CommitSizeText()
    {
        string text = this._sizeBox.Text.Trim();

        if (string.Equals(text, Strings.FitToWindow, StringComparison.CurrentCultureIgnoreCase))
        {
            this._fixedSize = null;
            this.Render();

            return;
        }

        if (ThumbicoSize.TryParse(text, out Size parsed))
        {
            this._fixedSize = parsed;
            this.Render();

            return;
        }

        this._sizeBox.Text = this._fixedSize is Size current
            ? ThumbicoSize.Format(current)
            : Strings.FitToWindow;
    }

    private void OnMakeBigger(object? sender, EventArgs e) => this.StepSize(SizeStep);

    private void OnMakeSmaller(object? sender, EventArgs e) => this.StepSize(1 / SizeStep);

    /// <summary>
    /// Scales the requested size, converting Fit to window into a fixed size on the first step.
    /// </summary>
    private void StepSize(double factor)
    {
        Size current = this._fixedSize ?? this.RequestedSize;
        int width = Math.Clamp((int)Math.Round(current.Width * factor), 1, ThumbicoSize.MaximumDimension);
        int height = Math.Clamp((int)Math.Round(current.Height * factor), 1, ThumbicoSize.MaximumDimension);

        this._fixedSize = new Size(width, height);
        this._sizeBox.Text = ThumbicoSize.Format(this._fixedSize.Value);
        this.Render();
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
        if (this._thumbico is null)
        {
            return;
        }

        using SaveFileDialog dialog = new()
        {
            AddExtension = true,
            Filter = Strings.SaveDialogFilter,
            FileName = Path.GetFileNameWithoutExtension(this._path) + "_thumbico",
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            this._thumbico.Save(dialog.FileName, ChooseFormat(dialog.FileName, dialog.FilterIndex));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            this._returnedLabel.Text = error.Message;
        }
    }

    /// <summary>
    /// Decides what to write, preferring the name the user typed over the filter they left selected.
    /// </summary>
    /// <remarks>
    /// The dialog appends the filter's extension when none was typed, so in the ordinary case these
    /// two agree and the filter decides. They disagree only when the name carries an extension of
    /// its own, and then the name has to win: every other program reads the file by its extension,
    /// so honouring the filter instead would write BMP bytes into something called .png.
    /// </remarks>
    private static ThumbicoFormat ChooseFormat(string fileName, int filterIndex)
        => Path.GetExtension(fileName).ToUpperInvariant() switch
        {
            ".PNG" => ThumbicoFormat.Png,
            ".BMP" => ThumbicoFormat.Bmp,
            ".GIF" => ThumbicoFormat.Gif,
            ".JPG" or ".JPEG" => ThumbicoFormat.Jpeg,
            ".TIF" or ".TIFF" => ThumbicoFormat.Tiff,
            _ => SaveFormats[filterIndex - 1],
        };

    /// <summary>
    /// Puts the image on the clipboard twice: as PNG bytes, which keep the alpha channel, and as a
    /// bitmap for applications that only read the legacy format.
    /// </summary>
    /// <remarks>
    /// The bitmap copy is flattened onto an opaque background first. Handing a transparent bitmap
    /// straight to SetImage does not merely lose the alpha, it corrupts the colours, because the
    /// clipboard converts it through a screen-compatible device bitmap.
    /// </remarks>
    private void OnCopy(object? sender, EventArgs e)
    {
        if (this._thumbico is null)
        {
            return;
        }

        // SetDataObject with copy: true renders every format before it returns, so the stream can go.
        using MemoryStream png = new();
        this._thumbico.Bitmap.Save(png, ImageFormat.Png);

        using Bitmap flattened = new(this._thumbico.Bitmap.Width, this._thumbico.Bitmap.Height);
        using (Graphics graphics = Graphics.FromImage(flattened))
        {
            graphics.Clear(this._canvas.SolidBackground ?? Color.White);
            graphics.DrawImageUnscaled(this._thumbico.Bitmap, 0, 0);
        }

        DataObject data = new();
        data.SetData("PNG", png);
        data.SetImage(flattened);
        Clipboard.SetDataObject(data, copy: true);
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
