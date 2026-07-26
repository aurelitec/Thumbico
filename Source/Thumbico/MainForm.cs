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

    private readonly SettingsStore _store = SettingsStore.CreateDefault();
    private readonly AppSettings _settings;
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
    private bool _nakedMode;
    private bool _settlingSizeText;

    internal MainForm(string? initialPath)
    {
        this.BuildLayout();

        // ApplySettings sets the size combo itself, so the default selection is not set separately;
        // doing both would ask the shell twice on startup.
        this._settings = this._store.Load();
        this.ApplySettings();
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
    /// The design size is written at 100 percent scale, so on a sufficiently scaled display it can
    /// exceed the work area and hang the status bar off the bottom of the desktop. Scaling by DPI
    /// makes that far less likely than the font scaling this once compensated for, but the ceiling is
    /// the screen rather than the scale factor, so the clamp stays. Both it and the first render
    /// belong here rather than in the constructor, because scaling has not been applied while that
    /// runs and Fit to window would size the first request against a canvas that is about to change.
    /// </remarks>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Only when the window is a normal window: assigning Size to a maximized form restores it.
        if (this.WindowState == FormWindowState.Normal)
        {
            Rectangle working = Screen.FromControl(this).WorkingArea;
            this.Size = new Size(
                Math.Min(this.Width, working.Width),
                Math.Min(this.Height, working.Height));
            this.Location = new Point(
                Math.Clamp(this.Left, working.Left, working.Right - this.Width),
                Math.Clamp(this.Top, working.Top, working.Bottom - this.Height));
        }

        if (!string.IsNullOrWhiteSpace(this._initialPath))
        {
            this.SetPath(this._initialPath);
        }
    }

    /// <summary>
    /// Puts the caret in the path box, which is what makes the Tab key work at all.
    /// </summary>
    /// <remarks>
    /// The toolbar is the only thing Tab visits, and a ToolStrip whose TabStop is off cannot be
    /// entered by Tab, so focus has to begin inside it. It also has to begin on one of the two hosted
    /// controls rather than on a button: seeded on a button, Tab does not move at all, which is the
    /// framework's own open accessibility bug, dotnet/winforms#5794. Measured both ways. This belongs
    /// here rather than in OnLoad because the window has to be on screen for the focus to stick.
    /// </remarks>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        this._pathBox.Focus();
    }

    /// <summary>
    /// Asks the shell again once the display scale changes, because the canvas has a new pixel size.
    /// </summary>
    /// <remarks>
    /// Only the request is refreshed. Rebuilding the chrome for a new scale is deliberately not
    /// attempted; see the display scale section of gui-design.md for what the framework gets wrong
    /// and why the interface is documented as needing a restart instead. Without this the request
    /// stays at the old canvas size, because the framework's own resize does not raise OnResizeEnd.
    /// </remarks>
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);

        if (this._fixedSize is null)
        {
            this.Render();
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
        else if (e.KeyCode == Keys.Escape && this._nakedMode)
        {
            e.SuppressKeyPress = true;
            this.SetNakedMode(naked: false);
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

    /// <summary>
    /// Restores what the user chose last time. The window bounds are only honoured when they still
    /// land on a connected screen, so a monitor that has gone away cannot hide the window.
    /// </summary>
    private void ApplySettings()
    {
        if (!this._settings.WindowBounds.IsEmpty
            && Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(this._settings.WindowBounds)))
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = this._settings.WindowBounds;
        }

        if (this._settings.WindowMaximized)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        // Color.Empty is what the converter yields for text it cannot read, and an empty colour is
        // not a colour the user ever chose, so it falls back to the checkerboard like a missing one.
        if (this._settings.BackgroundColor is Color background && background != Color.Empty)
        {
            this._canvas.SolidBackground = background;
            this._checkerboardItem.Checked = false;
            this._solidColorItem.Checked = true;
        }

        this._sizeBox.Text = string.IsNullOrWhiteSpace(this._settings.SizeSelection)
            ? Strings.FitToWindow
            : this._settings.SizeSelection;
        this.CommitSizeText();

        this._source = this._settings.Source;
        foreach (ToolStripMenuItem item in this._sourceItems)
        {
            item.Checked = (ThumbicoSource)item.Tag! == this._source;
        }

        this._options = this._settings.Options;
        foreach ((ToolStripMenuItem item, ThumbicoOptions flag) in this._optionItems)
        {
            item.Checked = this._options.HasFlag(flag);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        this._settings.WindowMaximized = this.WindowState == FormWindowState.Maximized;
        this._settings.WindowBounds = this.WindowState == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;
        this._settings.BackgroundColor = this._canvas.SolidBackground;
        this._settings.SizeSelection = this._sizeBox.Text;
        this._settings.Source = this._source;
        this._settings.Options = this._options;
        this._store.Save(this._settings);

        base.OnFormClosing(e);
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
    /// Reads the combo and settles the field to the one size format the rest of the interface uses.
    /// </summary>
    /// <remarks>
    /// Every route out settles the text, so the field reads the same whether a size was picked from
    /// the list, typed, stepped, or restored from settings. It used to be rewritten only when parsing
    /// failed, which left the shape an accident of how the value arrived while the status bar always
    /// spelled it out. Unparseable text falls back to the last good value rather than raising an
    /// error, because this is a live control and not a submitted form.
    /// </remarks>
    private void CommitSizeText()
    {
        // Settling the field assigns Text, and when that value matches a list entry the combo selects
        // it and raises SelectedIndexChanged, which lands back here. Documented: SelectedIndexChanged
        // fires for a programmatic change as readily as a user one. Unguarded, typing a size that
        // settles onto a listed one asks the shell twice.
        if (this._settlingSizeText)
        {
            return;
        }

        string text = this._sizeBox.Text.Trim();

        if (string.Equals(text, Strings.FitToWindow, StringComparison.CurrentCultureIgnoreCase))
        {
            this._fixedSize = null;
            this.SettleSizeText(Strings.FitToWindow);
            this.Render();

            return;
        }

        if (ThumbicoSize.TryParse(text, out Size parsed))
        {
            this._fixedSize = parsed;
            this.SettleSizeText(ThumbicoSize.Format(parsed));
            this.Render();

            return;
        }

        this.SettleSizeText(this._fixedSize is Size current
            ? ThumbicoSize.Format(current)
            : Strings.FitToWindow);
    }

    /// <summary>Writes the field without the write coming back as a fresh commit.</summary>
    private void SettleSizeText(string text)
    {
        this._settlingSizeText = true;
        this._sizeBox.Text = text;
        this._settlingSizeText = false;
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
        this.SettleSizeText(ThumbicoSize.Format(this._fixedSize.Value));
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

    /// <summary>
    /// Strips the window down to the image, keeping the title bar so it can still be moved.
    /// </summary>
    /// <remarks>
    /// The 1.0 and 1.5 shape, restored: hide the toolbar, the window buttons and the scrollbars, and
    /// leave the window exactly where it is. It never maximized and never went borderless - those
    /// were 2018's Fullscreen and 2021's Preview Mode, neither of which shipped. A window that stays
    /// a window can be put beside the thing you are comparing an icon against, which is the point.
    /// </remarks>
    private void OnNakedMode(object? sender, EventArgs e) => this.SetNakedMode(!this._nakedMode);

    private void SetNakedMode(bool naked)
    {
        this._nakedMode = naked;
        this._nakedModeItem.Checked = naked;
        this._toolStrip.Visible = !naked;
        this._statusStrip.Visible = !naked;
        this.ControlBox = !naked;
        this._canvas.AutoScroll = !naked;

        // The canvas has just grown into the space the chrome vacated, so the request has changed.
        // Layout runs synchronously here, so the new client size is already the one to measure.
        if (this._fixedSize is null)
        {
            this.Render();
        }
    }

    private void OnOnlineHelp(object? sender, EventArgs e)
        => Process.Start(new ProcessStartInfo(Urls.Help) { UseShellExecute = true });

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
