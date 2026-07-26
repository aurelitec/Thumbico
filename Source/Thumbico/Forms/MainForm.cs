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
    /// What Make Bigger multiplies the request by, and Make Smaller divides it by. A factor rather than
    /// a fixed step, since the shell often returns something smaller than asked and a fixed step
    /// against an ignored number can produce no visible change.
    /// </summary>
    private const double SizeStep = 1.25;

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

        // ApplySettings sets the size combo itself; setting a default as well asks the shell twice.
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
    /// Keeps the window inside the screen it opens on, then renders anything passed on the command line.
    /// </summary>
    /// <remarks>
    /// The design size is written at 100 percent scale, so a scaled display can push it past the work
    /// area. Neither step can move to the constructor: scaling has not run yet there, so Fit to window
    /// would measure a canvas that is about to change size.
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
    /// A ToolStrip with TabStop off cannot be entered by Tab, so focus has to start inside it - and on
    /// a hosted control, since seeding a button leaves Tab dead (dotnet/winforms#5794). Not in OnLoad:
    /// the window must be on screen for the focus to stick.
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
    /// Needed because the framework's own resize does not raise OnResizeEnd. Rebuilding the chrome for
    /// the new scale is deliberately not attempted; a scale change needs a restart.
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

    /// <summary>Yields Ctrl+C to a toolbar field with a selection; the menu's Copy takes it otherwise.</summary>
    /// <remarks>Skipping the base call is what frees the key; a focus-only test would lose Copy after a drop.</remarks>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.C)
            && ((this._pathBox.Focused && this._pathBox.TextBox.SelectionLength > 0)
                || (this._sizeBox.Focused && this._sizeBox.ComboBox.SelectionLength > 0)))
        {
            return false;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// F5 reloads and Escape leaves Naked Mode. Neither command has a menu item to carry a shortcut,
    /// so the form takes both keys itself.
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
    /// Adjustments are replayed rather than accumulated, so a rotation the user applied survives a
    /// change of size or source.
    /// </remarks>
    private void Render()
    {
        if (this._path is null)
        {
            return;
        }

        // An item is in play, so the bar reports the transaction from here on, failure included.
        this.SetStatusMessage(null);

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

        // Read before the adjustments touch it, or a rotation reads as though the shell had returned
        // the swapped dimensions.
        Size returned = loaded.Size;
        this.ApplyAdjustments(loaded);

        // Hand over the new bitmap before releasing the old one, so the canvas never holds a disposed
        // reference.
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
    /// Shows one message across the whole bar, or hands it back to the three indicators.
    /// </summary>
    /// <remarks>Hiding an indicator takes its divider with it, so a message leaves no sections.</remarks>
    private void SetStatusMessage(string? message)
    {
        this._messageLabel.Text = message ?? string.Empty;

        bool indicators = message is null;
        this._askedLabel.Visible = indicators;
        this._returnedLabel.Visible = indicators;
        this._kindLabel.Visible = indicators;
    }

    /// <summary>
    /// Reports a failure in the status bar rather than a dialog, since "this item has no thumbnail" is
    /// an ordinary answer for a live preview.
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
    /// Restores what the user chose last time. Saved bounds are honoured only if they still land on a
    /// connected screen, so a monitor that has gone away cannot hide the window.
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

        // Color.Empty is what the converter yields for unreadable text, so it means no choice at all.
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
        this.Text = $"{ItemPath.DisplayName(path)} - {Strings.AppName}";
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
    /// Reads the combo and settles the field to the one size format the interface uses everywhere.
    /// </summary>
    /// <remarks>
    /// Every route out settles the text, so the field reads the same however the size arrived.
    /// Unparseable text reverts to the last good value rather than raising an error, this being a live
    /// control and not a submitted form.
    /// </remarks>
    private void CommitSizeText()
    {
        // Settling assigns Text, which selects a matching list entry and so raises
        // SelectedIndexChanged - documented to fire for programmatic changes too - landing back here.
        // Unguarded, a typed size that settles onto a listed one asks the shell twice.
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
            FileName = ItemPath.DefaultSaveName(this._path),
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            this._thumbico.Save(dialog.FileName, ItemPath.FormatFrom(dialog.FileName, dialog.FilterIndex));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            this._returnedLabel.Text = error.Message;
        }
    }

    /// <summary>
    /// Puts the image on the clipboard twice: as PNG bytes, which keep the alpha channel, and as a
    /// bitmap for applications that only read the legacy format.
    /// </summary>
    /// <remarks>
    /// The bitmap copy must be flattened first: SetImage routes a transparent bitmap through a
    /// screen-compatible device bitmap, which corrupts the colours rather than merely dropping alpha.
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
    /// Hides the toolbar, the status bar, the window buttons and the scrollbars, and leaves the window
    /// where it is. Deliberately not maximized or borderless: a window that stays a window can be put
    /// beside whatever you are comparing an icon against.
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

        // The canvas has taken the space the chrome vacated, and layout ran synchronously above, so the
        // new client size is already the one to ask for.
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
