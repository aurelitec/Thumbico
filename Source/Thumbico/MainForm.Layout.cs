// Copyright (c) 2011-2026 Aurelitec <https://www.aurelitec.com>
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Thumbico;

/// <summary>
/// The hand-written construction half of the main window. No designer is involved, so none of the
/// designer's restrictions on the code apply.
/// </summary>
internal sealed partial class MainForm
{
    /// <summary>The narrowest the path box is allowed to get, at 100 percent display scale.</summary>
    private const int MinimumPathBoxWidth = 120;

    private static readonly string[] StandardSizes =
        ["16", "32", "48", "64", "128", "256", "512", "1024"];

    private ToolStrip _toolStrip = null!;
    private ToolStripButton _openButton = null!;
    private ToolStripTextBox _pathBox = null!;
    private ToolStripComboBox _sizeBox = null!;
    private ToolStripButton _refreshButton = null!;
    private ToolStripButton _menuButton = null!;
    private ThumbicoCanvas _canvas = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _askedLabel = null!;
    private ToolStripStatusLabel _returnedLabel = null!;
    private ToolStripStatusLabel _kindLabel = null!;
    private ContextMenuStrip _menu = null!;
    private ToolStripMenuItem _saveItem = null!;
    private ToolStripMenuItem _copyItem = null!;
    private ToolStripMenuItem _grayscaleItem = null!;
    private ToolStripMenuItem _rotateFlipItem = null!;
    private ToolStripMenuItem _checkerboardItem = null!;
    private ToolStripMenuItem _solidColorItem = null!;
    private ToolStripMenuItem _nakedModeItem = null!;
    private ToolStripMenuItem[] _sourceItems = null!;
    private (ToolStripMenuItem Item, ThumbicoOptions Flag)[] _optionItems = null!;

    /// <summary>
    /// The pixel size icons are drawn at, taken from the display scale rather than assumed. A
    /// bitmap rendered at a fixed 16 is half size on a 200 percent display.
    /// </summary>
    private int IconSize => 16 * this.DeviceDpi / 96;

    /// <summary>
    /// Releases what the control tree's own disposal does not reach: the current image, which the
    /// canvas borrows rather than owns, and the menu, which belongs to no control's collection.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._thumbico?.Dispose();
            this._menu?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void BuildLayout()
    {
        this.SuspendLayout();

        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(760, 640);
        this.MinimumSize = new Size(420, 320);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = Strings.AppName;
        this.AllowDrop = true;
        this.KeyPreview = true;

        this.BuildToolStrip();
        this.BuildStatusStrip();

        this._canvas = new ThumbicoCanvas { Dock = DockStyle.Fill, TabIndex = 1 };

        // Fill first, then the strips, so docking resolves the canvas last and it takes what is left.
        this.Controls.Add(this._canvas);
        this.Controls.Add(this._toolStrip);
        this.Controls.Add(this._statusStrip);

        this.BuildMenu();

        // Assigning the menu to the form is what makes its shortcuts work at all. Windows Forms
        // routes a command key through the focused control's ProcessCmdKey and up its parents, and
        // each one offers the key to its own ContextMenuStrip; a menu that is only ever Show()n is
        // in nobody's chain, so every accelerator on it is dead. Putting it on the form rather than
        // the canvas means the key is caught wherever focus happens to be, and right-clicking gets
        // handled natively, including the Menu key and Shift+F10.
        this.ContextMenuStrip = this._menu;

        this.ResumeLayout(performLayout: true);
    }

    /// <summary>
    /// Builds the single menu that the toolbar button and the canvas right-click both show.
    /// </summary>
    /// <remarks>
    /// One instance rather than two, because Thumbico has one object and every command acts on it,
    /// so a context menu scoped to what was clicked would hold the same items anyway.
    /// </remarks>
    private void BuildMenu()
    {
        ToolStripMenuItem open = this.BuildItem(
            Strings.MenuOpen, Glyphs.Open, Keys.Control | Keys.O, this.OnOpenClicked);
        // Windows Forms labels a shortcut from the Keys enum, which spells these two "Ctrl+Oemplus"
        // and "Ctrl+OemMinus". The display string is what the user reads; the key still binds.
        ToolStripMenuItem bigger = this.BuildItem(
            Strings.MenuMakeBigger, Glyphs.ZoomIn, Keys.Control | Keys.Oemplus, this.OnMakeBigger);
        bigger.ShortcutKeyDisplayString = Strings.ShortcutMakeBigger;

        ToolStripMenuItem smaller = this.BuildItem(
            Strings.MenuMakeSmaller, Glyphs.ZoomOut, Keys.Control | Keys.OemMinus, this.OnMakeSmaller);
        smaller.ShortcutKeyDisplayString = Strings.ShortcutMakeSmaller;

        this._rotateFlipItem = this.BuildItem(Strings.MenuRotateFlip, Glyphs.RotateFlip);
        this._rotateFlipItem.DropDownItems.AddRange(
        [
            this.BuildTaggedItem(Strings.MenuRotateLeft, Glyphs.RotateLeft, Keys.Alt | Keys.L,
                ThumbicoTransform.RotateLeft, this.OnRotate),
            this.BuildTaggedItem(Strings.MenuRotateRight, Glyphs.RotateRight, Keys.Alt | Keys.R,
                ThumbicoTransform.RotateRight, this.OnRotate),
            new ToolStripSeparator(),
            this.BuildTaggedItem(Strings.MenuFlipHorizontal, Glyphs.FlipHorizontal, Keys.Alt | Keys.H,
                ThumbicoTransform.FlipHorizontal, this.OnFlip),
            this.BuildTaggedItem(Strings.MenuFlipVertical, Glyphs.FlipVertical, Keys.Alt | Keys.V,
                ThumbicoTransform.FlipVertical, this.OnFlip),
        ]);

        this._grayscaleItem = this.BuildItem(Strings.MenuGrayscale, Glyphs.Grayscale, Keys.None, this.OnGrayscale);

        // Both act on a rendered image, so they stay unavailable until there is one.
        this._saveItem = this.BuildItem(Strings.MenuSaveImageAs, Glyphs.SaveAs, Keys.Control | Keys.S, this.OnSaveAs);
        this._saveItem.Enabled = false;
        this._copyItem = this.BuildItem(Strings.MenuCopy, Glyphs.Copy, Keys.Control | Keys.C, this.OnCopy);
        this._copyItem.Enabled = false;

        ToolStripMenuItem source = this.BuildItem(Strings.MenuSource, Glyphs.Source);
        this._sourceItems =
        [
            this.BuildTaggedItem(Strings.MenuSourceAuto, glyph: null, Keys.None,
                ThumbicoSource.Auto, this.OnSourceSelected),
            this.BuildTaggedItem(Strings.MenuSourceThumbnailOnly, glyph: null, Keys.None,
                ThumbicoSource.ThumbnailOnly, this.OnSourceSelected),
            this.BuildTaggedItem(Strings.MenuSourceIconOnly, glyph: null, Keys.None,
                ThumbicoSource.IconOnly, this.OnSourceSelected),
        ];
        source.DropDownItems.AddRange(this._sourceItems);

        ToolStripMenuItem advanced = this.BuildItem(Strings.MenuAdvanced, glyph: null);
        this._optionItems =
        [
            (this.BuildTaggedItem(Strings.MenuOptionAllowLargerSize, null, Keys.None,
                ThumbicoOptions.AllowLargerSize, this.OnOptionToggled), ThumbicoOptions.AllowLargerSize),
            (this.BuildTaggedItem(Strings.MenuOptionCropToSquare, null, Keys.None,
                ThumbicoOptions.CropToSquare, this.OnOptionToggled), ThumbicoOptions.CropToSquare),
            (this.BuildTaggedItem(Strings.MenuOptionWideAspect, null, Keys.None,
                ThumbicoOptions.WideAspect, this.OnOptionToggled), ThumbicoOptions.WideAspect),
            (this.BuildTaggedItem(Strings.MenuOptionIconBackground, null, Keys.None,
                ThumbicoOptions.IconBackground, this.OnOptionToggled), ThumbicoOptions.IconBackground),
            (this.BuildTaggedItem(Strings.MenuOptionScaleUp, null, Keys.None,
                ThumbicoOptions.ScaleUp, this.OnOptionToggled), ThumbicoOptions.ScaleUp),
        ];
        foreach ((ToolStripMenuItem item, ThumbicoOptions _) in this._optionItems)
        {
            item.CheckOnClick = true;
            advanced.DropDownItems.Add(item);
        }

        ToolStripMenuItem background = this.BuildItem(Strings.MenuBackground, Glyphs.Background);
        this._checkerboardItem = this.BuildItem(
            Strings.MenuBackgroundCheckerboard, null, Keys.None, this.OnCheckerboardSelected);
        this._checkerboardItem.Checked = true;
        this._solidColorItem = this.BuildItem(
            Strings.MenuBackgroundSolidColor, null, Keys.None, this.OnSolidColorSelected);
        background.DropDownItems.AddRange([this._checkerboardItem, this._solidColorItem]);

        this._nakedModeItem = this.BuildItem(
            Strings.MenuNakedMode, Glyphs.NakedMode, Keys.Control | Keys.N, this.OnNakedMode);

        this._menu = new ContextMenuStrip
        {
            // Same reason as the ToolStrip: the glyphs are rendered at the display's pixel size, and
            // the 16 by 16 default would resample them back down.
            ImageScalingSize = new Size(this.IconSize, this.IconSize),
        };
        this._menu.Items.AddRange(
        [
            open,
            new ToolStripSeparator(),
            bigger,
            smaller,
            new ToolStripSeparator(),
            this._rotateFlipItem,
            this._grayscaleItem,
            new ToolStripSeparator(),
            this._saveItem,
            this._copyItem,
            new ToolStripSeparator(),
            source,
            advanced,
            new ToolStripSeparator(),
            background,
            this._nakedModeItem,
            new ToolStripSeparator(),
            this.BuildItem(Strings.MenuOnlineHelp, Glyphs.Help, Keys.None, this.OnOnlineHelp),
            this.BuildItem(Strings.MenuAbout, Glyphs.About, Keys.None, this.OnAbout),
        ]);

        this._sourceItems[0].Checked = true;
    }

    private ToolStripMenuItem BuildItem(string text, string? glyph)
        => this.BuildItem(text, glyph, Keys.None, null);

    private ToolStripMenuItem BuildItem(string text, string? glyph, Keys shortcut, EventHandler? onClick)
    {
        ToolStripMenuItem item = new(text) { ShortcutKeys = shortcut };

        if (glyph is not null)
        {
            // MenuText rather than ControlText: the menu surface is not the toolbar surface.
            item.Image = Glyphs.Render(glyph, this.IconSize, SystemColors.MenuText);
        }

        if (onClick is not null)
        {
            item.Click += onClick;
        }

        return item;
    }

    private ToolStripMenuItem BuildTaggedItem(
        string text, string? glyph, Keys shortcut, object tag, EventHandler onClick)
    {
        ToolStripMenuItem item = this.BuildItem(text, glyph, shortcut, onClick);
        item.Tag = tag;

        return item;
    }

    private void BuildToolStrip()
    {
        this._openButton = this.BuildToolButton(Glyphs.Open, Strings.OpenButtonTooltip, this.OnOpenClicked);

        // AutoSize must go off first on both: while it is on, the item is sized from its content
        // and every Width assigned below is silently discarded at the next layout pass.
        this._pathBox = new ToolStripTextBox
        {
            AccessibleName = Strings.PathBoxAccessibleName,
            AutoSize = false,
            Width = 320,
        };
        this._pathBox.KeyDown += this.OnPathBoxKeyDown;

        this._sizeBox = new ToolStripComboBox
        {
            AccessibleName = Strings.SizeBoxAccessibleName,
            AutoSize = false,
            DropDownStyle = ComboBoxStyle.DropDown,
            Width = 110,
        };
        this._sizeBox.Items.Add(Strings.FitToWindow);
        foreach (string size in StandardSizes)
        {
            this._sizeBox.Items.Add(size);
        }

        this._sizeBox.SelectedIndexChanged += this.OnSizeBoxCommitted;
        this._sizeBox.LostFocus += this.OnSizeBoxCommitted;
        this._sizeBox.KeyDown += this.OnSizeBoxKeyDown;

        this._refreshButton = this.BuildToolButton(
            Glyphs.Refresh, Strings.RefreshButtonTooltip, this.OnRefreshClicked);
        this._menuButton = this.BuildToolButton(
            Glyphs.More, Strings.MenuButtonTooltip, this.OnMenuButtonClicked);

        this._toolStrip = new ToolStrip
        {
            // Five fixed items and one that stretches, so there is nothing worth hiding behind a
            // chevron. Leaving overflow on also loses a race: the strip decides what overflows
            // before StretchPathBox has shrunk the path box, so narrowing the window swept every
            // control except Open into the overflow menu.
            CanOverflow = false,
            GripStyle = ToolStripGripStyle.Hidden,

            // Match the size the glyphs were rendered at, or SizeToFit resamples them back down.
            ImageScalingSize = new Size(this.IconSize, this.IconSize),
            Padding = new Padding(6, 3, 6, 3),
            RenderMode = ToolStripRenderMode.System,
            TabIndex = 0,
            TabStop = true,
        };
        this._toolStrip.Items.AddRange(
            [this._openButton, this._pathBox, this._sizeBox, this._refreshButton, this._menuButton]);

        // ToolStrip has no notion of a stretching item, so the path box is sized by hand.
        this._toolStrip.SizeChanged += (_, _) => this.StretchPathBox();
    }

    private void BuildStatusStrip()
    {
        this._askedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        this._returnedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        this._kindLabel = new ToolStripStatusLabel();

        this._statusStrip = new StatusStrip { SizingGrip = true };
        this._statusStrip.Items.AddRange([this._askedLabel, this._returnedLabel, this._kindLabel]);
    }

    private void StretchPathBox()
    {
        int used = 0;
        foreach (ToolStripItem item in this._toolStrip.Items)
        {
            if (item != this._pathBox)
            {
                used += item.Width + item.Margin.Horizontal;
            }
        }

        int available = this._toolStrip.ClientSize.Width
            - used
            - this._pathBox.Margin.Horizontal
            - this._toolStrip.Padding.Horizontal;

        this._pathBox.Width = Math.Max(MinimumPathBoxWidth * this.DeviceDpi / 96, available);
    }

    private ToolStripButton BuildToolButton(string glyph, string tooltip, EventHandler onClick)
    {
        ToolStripButton button = new()
        {
            AccessibleName = tooltip,
            AutoToolTip = false,
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Image = Glyphs.Render(glyph, this.IconSize, SystemColors.ControlText),
            ToolTipText = tooltip,
        };
        button.Click += onClick;

        return button;
    }
}
