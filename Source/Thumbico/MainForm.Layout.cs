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
    /// Worth setting because the framework leaves next to nothing: a button's default margin is zero
    /// horizontally and a hosted text box or combo gets two raw pixels, so at any scale above 100 the
    /// items sit flush and the path box and the combo read as one joined box.
    /// </remarks>
    private const int ToolbarItemSpacing = 4;

    /// <summary>Space each side of a status bar pane's text, at 100 percent display scale.</summary>
    /// <remarks>
    /// A pane is given none at all, so its text sits hard against the divider line beside it and one
    /// pixel from the window edge. This separates both.
    /// </remarks>
    private const int StatusPaneHorizontalPadding = 8;

    /// <summary>Space above and below a status bar pane's text, at 100 percent display scale.</summary>
    /// <remarks>
    /// Sets the bar's height, which the framework otherwise leaves at 23 logical pixels - noticeably
    /// tighter than the status bars Windows' own applications draw.
    /// </remarks>
    private const int StatusPaneVerticalPadding = 3;

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

    /// <summary>The two images an item with no icon of its own alternates between.</summary>
    /// <remarks>
    /// One tick and one empty box, shared by every such item rather than built per item or per toggle.
    /// Both are needed at the full image size: the blank one because a row only takes the image
    /// column's height if it has an image at all, and the tick because the framework's own check is
    /// stretched to that column and arrives blurry.
    /// </remarks>
    private Bitmap _menuCheckImage = null!;
    private Bitmap _menuBlankImage = null!;

    /// <summary>
    /// The size toolbar icons are drawn at, taken from the display scale rather than assumed.
    /// </summary>
    /// <remarks>
    /// Deliberately larger than the menu's, and larger than the 16 a ToolStrip defaults to, because
    /// these buttons carry no text at all and the icon is the whole label. Measured against Windows'
    /// own toolbars, 16 left the sparser glyphs covering noticeably less of their box; the icon set
    /// does not inset every glyph equally, so raising the box is the only safe way to fill it.
    /// </remarks>
    private int ToolbarIconSize => 20 * this.DeviceDpi / 96;

    /// <summary>
    /// The box a menu image occupies, taken from the display scale rather than assumed.
    /// </summary>
    /// <remarks>
    /// This is what sets the row height, because a row is at least as tall as the image column, and it
    /// is the only lever that opens the rows up while keeping their contents centred. Both documented
    /// alternatives were tried and measured, and both fail the same way: item Padding, and AutoSize off
    /// with an explicit Size, each inflate the item without the drop-down recomputing its text
    /// rectangle, so the text and shortcut end up against the top of the row - by the same offsets to
    /// the pixel. Was 16, on the reasoning that taller rows were to be avoided; once they became the
    /// goal that reasoning inverted. Measured, this gives a 26 logical row where 16 gave 18.
    /// </remarks>
    private int MenuImageSize => 24 * this.DeviceDpi / 96;

    /// <summary>
    /// The size a menu glyph is drawn at inside that box, taken from the display scale.
    /// </summary>
    /// <remarks>
    /// Deliberately smaller than the box, and left at what Windows menus use. Separating the two is
    /// what buys open rows without oversized icons: the row is as tall as the image column, while the
    /// ink stays 16 and the surplus is transparent padding. Tying them together made the rows right and
    /// the icons visibly too large.
    /// </remarks>
    private int MenuGlyphSize => 16 * this.DeviceDpi / 96;

    /// <summary>
    /// The size a check mark is drawn at inside the image box, taken from the display scale.
    /// </summary>
    /// <remarks>
    /// Smaller than the icons on purpose, and not because it is larger - measured at a shared size its
    /// ink is 24.8 by 18.4 against the folder's 27.3 by 22.4, the smallest ink area in the set. The tick
    /// is a single stroke spanning most of its box while every other glyph here is a closed form, so it
    /// reads heavier at the same size. Two logical pixels down settles it against the icons above.
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

        // Scale by display resolution, not by font metrics. Font mode derives its factor from average
        // character size, which does not grow evenly in both directions, so widths and heights end up
        // scaled differently - a squeezed toolbar and a window taller than it should be. Dpi mode is
        // linear by definition, and the documentation recommends it for graphics-based applications
        // and against Font mode where the absolute size matters. Here it matters: in Fit to window
        // mode the canvas size is the size asked of the shell. Measurements in gui-design.md.
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
        this._menuCheckImage = Glyphs.Render(
            Glyphs.Checkmark, this.MenuCheckGlyphSize, this.MenuImageSize, SystemColors.MenuText);
        this._menuBlankImage = new Bitmap(this.MenuImageSize, this.MenuImageSize);

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
            // The box the glyphs are padded out to, which is larger than the glyphs themselves and
            // larger than the toolbar's. This is what gives the rows their height.
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
            // No icon of its own, so it shows the tick when checked and an empty box otherwise. Bound
            // to the item's own event rather than updated at each call site, of which there are nine.
            // The empty box is not decoration: a row only takes the image column's height if it has an
            // image at all, so without it this row alone would stay at the tighter text height.
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
        foreach (string size in StandardSizes)
        {
            this._sizeBox.Items.Add(size);
        }

        this._sizeBox.SelectedIndexChanged += this.OnSizeBoxCommitted;
        this._sizeBox.LostFocus += this.OnSizeBoxCommitted;
        this._sizeBox.KeyDown += this.OnSizeBoxKeyDown;

        // Give the path box the combo's height so the two read as one pair. A floor rather than an
        // assigned Height on purpose: assigning Height needs TextBoxBase.AutoSize off, and a
        // single-line text box then draws its text at the top, having no vertical alignment of its
        // own. A minimum leaves AutoSize on, so the text stays centred. Height only - the combo's
        // full Size would also impose its width as a floor, quietly competing with StretchPathBox.
        // The combo reports its height correctly here, before either has been laid out, because that
        // height comes from the font.
        this._pathBox.TextBox.MinimumSize = new Size(0, this._sizeBox.Height);

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

            // Match the size the glyphs were drawn at, which the 16 the default resolves to would
            // otherwise shrink them back to.
            ImageScalingSize = new Size(this.ToolbarIconSize, this.ToolbarIconSize),
            Padding = new Padding(6, 3, 6, 3),
            RenderMode = ToolStripRenderMode.System,
            TabIndex = 0,

            // Left at the default false on purpose. True means what its documentation says - one Tab
            // enters the strip, the arrow keys move within it, and the next Tab leaves - which strands
            // the path box and the size combo, because an arrow key inside a text box moves the caret
            // rather than moving on. False instead lets Tab step through the items one at a time.
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
        // Divider on each pane's leading edge, so the group is bounded where it begins rather than
        // trailing off into the sizing grip.
        this._askedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };
        this._returnedLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };
        this._kindLabel = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Left };

        this._statusStrip = new StatusStrip { SizingGrip = true };

        // The empty first pane is what puts the rest against the right edge. Windows' status bar model
        // is a stretching message line first and the indicators after it, and all three of ours are
        // indicators rather than messages. Spring is the StatusStrip's own way to hand one pane the
        // slack; ToolStripItem.Alignment cannot do it, because a StatusStrip lays out as a table and
        // a table ignores Alignment.
        this._statusStrip.Items.AddRange(
        [
            new ToolStripStatusLabel { Spring = true },
            this._askedLabel,
            this._returnedLabel,
            this._kindLabel,
        ]);

        // Scaled by hand, as every item value must be: autoscale reaches a StatusStrip's own padding
        // but nothing belonging to the items inside it.
        int horizontal = StatusPaneHorizontalPadding * this.DeviceDpi / 96;
        int vertical = StatusPaneVerticalPadding * this.DeviceDpi / 96;

        foreach (ToolStripItem pane in this._statusStrip.Items)
        {
            pane.Padding = new Padding(horizontal, vertical, horizontal, vertical);
        }
    }

    /// <summary>Separates the toolbar items, which the framework's own margins barely do.</summary>
    /// <remarks>
    /// The gap goes on the right of each item only, so two neighbours are one spacing apart rather
    /// than two, and the strip's own padding still owns the outer edges. Each item keeps whatever
    /// vertical margin its type chose, since that is what positions it in the row, and the last item
    /// keeps no gap because there is nothing after it to be separated from. Scaled by hand: autoscale
    /// reaches a ToolStrip's padding but not a ToolStripItem's, which is not a control.
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
