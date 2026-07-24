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

    /// <summary>
    /// The pixel size icons are drawn at, taken from the display scale rather than assumed. A
    /// bitmap rendered at a fixed 16 is half size on a 200 percent display.
    /// </summary>
    private int IconSize => 16 * this.DeviceDpi / 96;

    /// <summary>
    /// Releases what the control tree's own disposal does not reach. Task 8 adds the current
    /// thumbico here once there is one to release.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
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
        this._canvas.MouseUp += this.OnCanvasMouseUp;

        // Fill first, then the strips, so docking resolves the canvas last and it takes what is left.
        this.Controls.Add(this._canvas);
        this.Controls.Add(this._toolStrip);
        this.Controls.Add(this._statusStrip);

        this.BuildMenu();

        this.ResumeLayout(performLayout: true);
    }

    /// <summary>
    /// Placeholder until Task 7 builds the real menu.
    /// </summary>
    private void BuildMenu()
    {
        this._menu = new ContextMenuStrip();
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
