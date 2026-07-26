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

    /// <summary>
    /// Space around a toolbar button's icon, at 100 percent display scale. A ToolStripButton adds
    /// none of its own vertically, which left the icon nearly touching the button's edge.
    /// </summary>
    private const int ToolbarButtonPadding = 5;

    /// <summary>The gap between neighbouring toolbar items, at 100 percent display scale.</summary>
    /// <remarks>
    /// Needed because a button's default horizontal margin is zero and a hosted control's is two raw
    /// pixels, so above 100 percent the items sit flush and the two fields read as one box.
    /// </remarks>
    private const int ToolbarItemSpacing = 4;

    /// <summary>Space each side of a status bar pane's text, at 100 percent display scale.</summary>
    /// <remarks>A pane gets none, so its text otherwise touches the divider beside it.</remarks>
    private const int StatusPaneHorizontalPadding = 8;

    /// <summary>Space above and below a status bar pane's text, at 100 percent display scale.</summary>
    /// <remarks>Sets the bar's height, which the framework leaves tighter than Windows' own.</remarks>
    private const int StatusPaneVerticalPadding = 3;

    /// <summary>The standard sizes the combo offers, written as the interface displays them.</summary>
    /// <remarks>
    /// Every icon size the shell itself uses, then doubled twice for the thumbnail range. Each entry has
    /// to read exactly as <see cref="ThumbicoSize.Format"/> writes it, or picking one settles the field
    /// to a different string than the list shows.
    /// </remarks>
    private static readonly string[] StandardSizes =
    [
        "16 x 16", "24 x 24", "32 x 32", "48 x 48", "64 x 64", "96 x 96",
        "128 x 128", "256 x 256", "512 x 512", "1024 x 1024", "2048 x 2048",
    ];

    private ToolStrip _toolStrip = null!;
    private ToolStripButton _openButton = null!;
    private ToolStripTextBox _pathBox = null!;
    private ToolStripComboBox _sizeBox = null!;
    private ToolStripButton _refreshButton = null!;
    private ToolStripButton _menuButton = null!;
    private ThumbicoCanvas _canvas = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _messageLabel = null!;
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

    /// <summary>The tick and the empty box an item with no icon of its own alternates between.</summary>
    /// <remarks>
    /// Shared rather than built per item. The blank one is load bearing: a row only takes the image
    /// column's height if the item has an image at all.
    /// </remarks>
    private Bitmap _menuCheckImage = null!;
    private Bitmap _menuBlankImage = null!;

    /// <summary>
    /// The size toolbar icons are drawn at, taken from the display scale rather than assumed.
    /// </summary>
    /// <remarks>Larger than the menu's, because these buttons have no text and the icon is the label.</remarks>
    private int ToolbarIconSize => 20 * this.DeviceDpi / 96;

    /// <summary>
    /// The box a menu image occupies, taken from the display scale rather than assumed.
    /// </summary>
    /// <remarks>
    /// This sets the row height, and is the only lever that does so while keeping a row's contents
    /// centred: growing the item instead leaves its text against the top, because the drop-down does not
    /// recompute the text rectangle.
    /// </remarks>
    private int MenuImageSize => 24 * this.DeviceDpi / 96;

    /// <summary>
    /// The size a menu glyph is drawn at inside that box, taken from the display scale.
    /// </summary>
    /// <remarks>
    /// Kept smaller than the box on purpose: the surplus is transparent padding, which is what buys open
    /// rows without oversized icons.
    /// </remarks>
    private int MenuGlyphSize => 16 * this.DeviceDpi / 96;

    /// <summary>
    /// The size a check mark is drawn at inside the image box, taken from the display scale.
    /// </summary>
    /// <remarks>
    /// Smaller than the icons because a tick is one stroke among closed forms and reads heavier at the
    /// same size.
    /// </remarks>
    private int MenuCheckGlyphSize => 14 * this.DeviceDpi / 96;

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
            this._menuCheckImage?.Dispose();
            this._menuBlankImage?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void BuildLayout()
    {
        this.SuspendLayout();

        // Dpi rather than Font mode. Font mode's factor comes from average character size, which does
        // not grow evenly in both directions, so widths and heights end up on different factors. Here
        // the absolute size matters: in Fit to window mode the canvas size is what is asked of the shell.
        this.AutoScaleDimensions = new SizeF(96F, 96F);
        this.AutoScaleMode = AutoScaleMode.Dpi;
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

        // This is what makes the menu's shortcuts work at all: a command key is offered to each
        // ContextMenuStrip up the focus chain, so a menu that is only ever Show()n has dead
        // accelerators. On the form rather than the canvas, so the key is caught wherever focus is.
        this.ContextMenuStrip = this._menu;

        this.ResumeLayout(performLayout: true);
    }

    /// <summary>
    /// Builds the single menu that the toolbar button and the canvas right-click both show.
    /// </summary>
    /// <remarks>
    /// One instance, not two: the app has a single object and every command acts on it, so a menu scoped
    /// to what was clicked would hold the same items.
    /// </remarks>
    private void BuildMenu()
    {
        this._menuCheckImage = Glyphs.Render(
            Glyphs.Checkmark, this.MenuCheckGlyphSize, this.MenuImageSize, SystemColors.MenuText);
        this._menuBlankImage = new Bitmap(this.MenuImageSize, this.MenuImageSize);

        ToolStripMenuItem open = this.BuildItem(
            Strings.MenuOpen, Glyphs.Open, Keys.Control | Keys.O, this.OnOpenClicked);
        // The display string is only the label; the key still binds. See Strings for why it is needed.
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
            // The padded box rather than the glyph size, which is what gives the rows their height.
            ImageScalingSize = new Size(this.MenuImageSize, this.MenuImageSize),
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

    /// <summary>Swaps an iconless item between the tick and the empty box as it is checked.</summary>
    private void OnMenuItemCheckedChanged(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem item)
        {
            item.Image = item.Checked ? this._menuCheckImage : this._menuBlankImage;
        }
    }

    private ToolStripMenuItem BuildItem(string text, string? glyph)
        => this.BuildItem(text, glyph, Keys.None, null);

    private ToolStripMenuItem BuildItem(string text, string? glyph, Keys shortcut, EventHandler? onClick)
    {
        ToolStripMenuItem item = new(text) { ShortcutKeys = shortcut };

        if (glyph is null)
        {
            // No icon of its own, so it shows the tick when checked and the empty box otherwise. Bound
            // to the item's event rather than to each of the nine places the check state is set.
            item.CheckedChanged += this.OnMenuItemCheckedChanged;
            item.Image = this._menuBlankImage;
        }
        else
        {
            // MenuText rather than ControlText: the menu surface is not the toolbar surface.
            item.Image = Glyphs.Render(glyph, this.MenuGlyphSize, this.MenuImageSize, SystemColors.MenuText);
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

            // Let the strip draw the frame instead of the text box drawing its own, so the path box
            // and the combo beside it are bordered alike.
            BorderStyle = BorderStyle.None,
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
        this._sizeBox.Items.AddRange(StandardSizes);

        this._sizeBox.SelectedIndexChanged += this.OnSizeBoxCommitted;
        this._sizeBox.LostFocus += this.OnSizeBoxCommitted;
        this._sizeBox.KeyDown += this.OnSizeBoxKeyDown;

        // Give the path box the combo's height so the two read as a pair. A floor, not an assigned
        // Height: that needs AutoSize off, and a single-line text box then draws its text at the top,
        // having no vertical alignment. Height only - the full Size would impose a width floor too,
        // competing with StretchPathBox.
        this._pathBox.TextBox.MinimumSize = new Size(0, this._sizeBox.Height);

        this._refreshButton = this.BuildToolButton(
            Glyphs.Refresh, Strings.RefreshButtonTooltip, this.OnRefreshClicked);
        this._menuButton = this.BuildToolButton(
            Glyphs.More, Strings.MenuButtonTooltip, this.OnMenuButtonClicked);

        this._toolStrip = new ToolStrip
        {
            // Off because the strip decides what overflows before StretchPathBox has shrunk the path
            // box, which swept everything but Open into the chevron as the window narrowed.
            CanOverflow = false,
            GripStyle = ToolStripGripStyle.Hidden,

            // Match the size the glyphs were drawn at, or the default 16 shrinks them back.
            ImageScalingSize = new Size(this.ToolbarIconSize, this.ToolbarIconSize),
            Padding = new Padding(6, 3, 6, 3),
            RenderMode = ToolStripRenderMode.System,
            TabIndex = 0,

            // False on purpose. True gives arrow-key navigation within the strip, which strands the two
            // hosted fields, since an arrow key there moves the caret. False lets Tab step item to item.
            TabStop = false,
        };
        this._toolStrip.Items.AddRange(
            [this._openButton, this._pathBox, this._sizeBox, this._refreshButton, this._menuButton]);

        this.SpaceToolStripItems();

        // ToolStrip has no notion of a stretching item, so the path box is sized by hand.
        this._toolStrip.SizeChanged += (_, _) => this.StretchPathBox();
    }

    private void BuildStatusStrip()
    {
        // Divider on each leading edge, so the group is bounded where it begins rather than trailing
        // off into the sizing grip.
        this._askedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };
        this._returnedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };
        this._kindLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };

        // A message line first, indicators after it, which is Windows' own status bar model. Spring is
        // how a StatusStrip hands one pane the slack; Alignment cannot, because a table layout ignores
        // it. Alignment set explicitly so it does not rest on a default.
        this._messageLabel = new ToolStripStatusLabel
        {
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        this._statusStrip = new StatusStrip { SizingGrip = true };
        this._statusStrip.Items.AddRange(
        [
            this._messageLabel,
            this._askedLabel,
            this._returnedLabel,
            this._kindLabel,
        ]);

        // Scaled by hand, as every item value must be: autoscale reaches a strip's own padding but
        // nothing on the items inside it.
        int horizontal = StatusPaneHorizontalPadding * this.DeviceDpi / 96;
        int vertical = StatusPaneVerticalPadding * this.DeviceDpi / 96;

        foreach (ToolStripItem pane in this._statusStrip.Items)
        {
            pane.Padding = new Padding(horizontal, vertical, horizontal, vertical);
        }

        // Nothing is loaded yet, so the bar carries the prompt instead of a shell transaction.
        this.SetStatusMessage(Strings.DropPrompt);
    }

    /// <summary>Separates the toolbar items, which the framework's own margins barely do.</summary>
    /// <remarks>
    /// On the right of each item only, so neighbours sit one gap apart rather than two and the strip's
    /// padding keeps the outer edges. Each item keeps its own vertical margin, which is what positions
    /// it in the row.
    /// </remarks>
    private void SpaceToolStripItems()
    {
        int spacing = ToolbarItemSpacing * this.DeviceDpi / 96;

        foreach (ToolStripItem item in this._toolStrip.Items)
        {
            bool last = item == this._toolStrip.Items[^1];
            item.Margin = new Padding(0, item.Margin.Top, last ? 0 : spacing, item.Margin.Bottom);
        }
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
        int padding = ToolbarButtonPadding * this.DeviceDpi / 96;

        ToolStripButton button = new()
        {
            AccessibleName = tooltip,
            AutoToolTip = false,
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Image = Glyphs.Render(glyph, this.ToolbarIconSize, SystemColors.ControlText),
            Padding = new Padding(padding),
            ToolTipText = tooltip,
        };
        button.Click += onClick;

        return button;
    }
}
