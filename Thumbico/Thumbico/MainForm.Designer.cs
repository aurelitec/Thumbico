namespace Thumbico
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyEditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeToFitImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biggerSizeOkImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryOnlyImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconOnlyImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thumbnailOnlyImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inCacheOnlyImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cropToSquareImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wideThumbnailsImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconBackgroundImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleUpImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.thumbiconPictureBox = new System.Windows.Forms.PictureBox();
            this.thumbiconPanel = new System.Windows.Forms.Panel();
            this.toolbarTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.desiredSizeFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.desiredSizeLabel = new System.Windows.Forms.Label();
            this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.xLabel = new System.Windows.Forms.Label();
            this.heightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.returnedSizeLabel = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbiconPictureBox)).BeginInit();
            this.thumbiconPanel.SuspendLayout();
            this.toolbarTableLayoutPanel.SuspendLayout();
            this.desiredSizeFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.imageMenuItem,
            this.viewMenuItem,
            this.helpMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1002, 33);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileMenuItem,
            this.toolStripSeparator1,
            this.exitFileMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileMenuItem.Text = "&File";
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFileMenuItem.Size = new System.Drawing.Size(235, 34);
            this.openFileMenuItem.Text = "&Open...";
            this.openFileMenuItem.Click += new System.EventHandler(this.OpenFileMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
            // 
            // exitFileMenuItem
            // 
            this.exitFileMenuItem.Name = "exitFileMenuItem";
            this.exitFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitFileMenuItem.Size = new System.Drawing.Size(235, 34);
            this.exitFileMenuItem.Text = "E&xit";
            this.exitFileMenuItem.Click += new System.EventHandler(this.ExitFileMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyEditMenuItem});
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(58, 29);
            this.editMenuItem.Text = "&Edit";
            // 
            // copyEditMenuItem
            // 
            this.copyEditMenuItem.Name = "copyEditMenuItem";
            this.copyEditMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyEditMenuItem.Size = new System.Drawing.Size(218, 34);
            this.copyEditMenuItem.Text = "&Copy";
            this.copyEditMenuItem.Click += new System.EventHandler(this.CopyEditMenuItem_Click);
            // 
            // imageMenuItem
            // 
            this.imageMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resizeToFitImageMenuItem,
            this.biggerSizeOkImageMenuItem,
            this.memoryOnlyImageMenuItem,
            this.iconOnlyImageMenuItem,
            this.thumbnailOnlyImageMenuItem,
            this.inCacheOnlyImageMenuItem,
            this.cropToSquareImageMenuItem,
            this.wideThumbnailsImageMenuItem,
            this.iconBackgroundImageMenuItem,
            this.scaleUpImageMenuItem});
            this.imageMenuItem.Name = "imageMenuItem";
            this.imageMenuItem.Size = new System.Drawing.Size(78, 29);
            this.imageMenuItem.Text = "&Image";
            // 
            // resizeToFitImageMenuItem
            // 
            this.resizeToFitImageMenuItem.Checked = true;
            this.resizeToFitImageMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resizeToFitImageMenuItem.Name = "resizeToFitImageMenuItem";
            this.resizeToFitImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.resizeToFitImageMenuItem.Text = "Resize To Fit";
            this.resizeToFitImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // biggerSizeOkImageMenuItem
            // 
            this.biggerSizeOkImageMenuItem.Name = "biggerSizeOkImageMenuItem";
            this.biggerSizeOkImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.biggerSizeOkImageMenuItem.Text = "Bigger Size Ok";
            this.biggerSizeOkImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // memoryOnlyImageMenuItem
            // 
            this.memoryOnlyImageMenuItem.Name = "memoryOnlyImageMenuItem";
            this.memoryOnlyImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.memoryOnlyImageMenuItem.Text = "Memory Only";
            this.memoryOnlyImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // iconOnlyImageMenuItem
            // 
            this.iconOnlyImageMenuItem.Name = "iconOnlyImageMenuItem";
            this.iconOnlyImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.iconOnlyImageMenuItem.Text = "Icon Only";
            this.iconOnlyImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // thumbnailOnlyImageMenuItem
            // 
            this.thumbnailOnlyImageMenuItem.Name = "thumbnailOnlyImageMenuItem";
            this.thumbnailOnlyImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.thumbnailOnlyImageMenuItem.Text = "Thumbnail Only";
            this.thumbnailOnlyImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // inCacheOnlyImageMenuItem
            // 
            this.inCacheOnlyImageMenuItem.Name = "inCacheOnlyImageMenuItem";
            this.inCacheOnlyImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.inCacheOnlyImageMenuItem.Text = "In Cache Only";
            this.inCacheOnlyImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // cropToSquareImageMenuItem
            // 
            this.cropToSquareImageMenuItem.Name = "cropToSquareImageMenuItem";
            this.cropToSquareImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.cropToSquareImageMenuItem.Text = "Crop To Square";
            this.cropToSquareImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // wideThumbnailsImageMenuItem
            // 
            this.wideThumbnailsImageMenuItem.Name = "wideThumbnailsImageMenuItem";
            this.wideThumbnailsImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.wideThumbnailsImageMenuItem.Text = "Wide Thumbnails";
            this.wideThumbnailsImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // iconBackgroundImageMenuItem
            // 
            this.iconBackgroundImageMenuItem.Name = "iconBackgroundImageMenuItem";
            this.iconBackgroundImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.iconBackgroundImageMenuItem.Text = "Icon Background";
            this.iconBackgroundImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // scaleUpImageMenuItem
            // 
            this.scaleUpImageMenuItem.Name = "scaleUpImageMenuItem";
            this.scaleUpImageMenuItem.Size = new System.Drawing.Size(251, 34);
            this.scaleUpImageMenuItem.Text = "Scale Up";
            this.scaleUpImageMenuItem.Click += new System.EventHandler(this.FlagsImageMenuItems_Click);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundColorToolStripMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewMenuItem.Text = "&View";
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(269, 34);
            this.backgroundColorToolStripMenuItem.Text = "&Background Color...";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.BackgroundColorViewMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(65, 29);
            this.helpMenuItem.Text = "&Help";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // thumbiconPictureBox
            // 
            this.thumbiconPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.thumbiconPictureBox.Location = new System.Drawing.Point(373, 182);
            this.thumbiconPictureBox.Name = "thumbiconPictureBox";
            this.thumbiconPictureBox.Size = new System.Drawing.Size(256, 256);
            this.thumbiconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.thumbiconPictureBox.TabIndex = 1;
            this.thumbiconPictureBox.TabStop = false;
            // 
            // thumbiconPanel
            // 
            this.thumbiconPanel.AutoScroll = true;
            this.thumbiconPanel.BackColor = System.Drawing.SystemColors.Control;
            this.thumbiconPanel.Controls.Add(this.thumbiconPictureBox);
            this.thumbiconPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thumbiconPanel.Location = new System.Drawing.Point(0, 92);
            this.thumbiconPanel.Name = "thumbiconPanel";
            this.thumbiconPanel.Size = new System.Drawing.Size(1002, 620);
            this.thumbiconPanel.TabIndex = 2;
            // 
            // toolbarTableLayoutPanel
            // 
            this.toolbarTableLayoutPanel.AutoSize = true;
            this.toolbarTableLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolbarTableLayoutPanel.ColumnCount = 2;
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.toolbarTableLayoutPanel.Controls.Add(this.desiredSizeFlowLayoutPanel, 0, 0);
            this.toolbarTableLayoutPanel.Controls.Add(this.returnedSizeLabel, 1, 0);
            this.toolbarTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbarTableLayoutPanel.Location = new System.Drawing.Point(0, 33);
            this.toolbarTableLayoutPanel.Name = "toolbarTableLayoutPanel";
            this.toolbarTableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.toolbarTableLayoutPanel.RowCount = 1;
            this.toolbarTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolbarTableLayoutPanel.Size = new System.Drawing.Size(1002, 59);
            this.toolbarTableLayoutPanel.TabIndex = 3;
            // 
            // desiredSizeFlowLayoutPanel
            // 
            this.desiredSizeFlowLayoutPanel.AutoSize = true;
            this.desiredSizeFlowLayoutPanel.Controls.Add(this.desiredSizeLabel);
            this.desiredSizeFlowLayoutPanel.Controls.Add(this.widthNumericUpDown);
            this.desiredSizeFlowLayoutPanel.Controls.Add(this.xLabel);
            this.desiredSizeFlowLayoutPanel.Controls.Add(this.heightNumericUpDown);
            this.desiredSizeFlowLayoutPanel.Location = new System.Drawing.Point(11, 11);
            this.desiredSizeFlowLayoutPanel.Name = "desiredSizeFlowLayoutPanel";
            this.desiredSizeFlowLayoutPanel.Size = new System.Drawing.Size(338, 37);
            this.desiredSizeFlowLayoutPanel.TabIndex = 3;
            // 
            // desiredSizeLabel
            // 
            this.desiredSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.desiredSizeLabel.AutoSize = true;
            this.desiredSizeLabel.Location = new System.Drawing.Point(3, 6);
            this.desiredSizeLabel.Name = "desiredSizeLabel";
            this.desiredSizeLabel.Size = new System.Drawing.Size(106, 25);
            this.desiredSizeLabel.TabIndex = 0;
            this.desiredSizeLabel.Text = "Desired size";
            // 
            // widthNumericUpDown
            // 
            this.widthNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.widthNumericUpDown.Location = new System.Drawing.Point(115, 3);
            this.widthNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.widthNumericUpDown.Name = "widthNumericUpDown";
            this.widthNumericUpDown.Size = new System.Drawing.Size(94, 31);
            this.widthNumericUpDown.TabIndex = 1;
            this.widthNumericUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.widthNumericUpDown.ValueChanged += new System.EventHandler(this.WidthNumericUpDown_ValueChanged);
            // 
            // xLabel
            // 
            this.xLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(215, 6);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(20, 25);
            this.xLabel.TabIndex = 3;
            this.xLabel.Text = "x";
            this.xLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // heightNumericUpDown
            // 
            this.heightNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.heightNumericUpDown.Location = new System.Drawing.Point(241, 3);
            this.heightNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.heightNumericUpDown.Name = "heightNumericUpDown";
            this.heightNumericUpDown.Size = new System.Drawing.Size(94, 31);
            this.heightNumericUpDown.TabIndex = 4;
            this.heightNumericUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // returnedSizeLabel
            // 
            this.returnedSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.returnedSizeLabel.AutoSize = true;
            this.returnedSizeLabel.Location = new System.Drawing.Point(991, 17);
            this.returnedSizeLabel.Name = "returnedSizeLabel";
            this.returnedSizeLabel.Size = new System.Drawing.Size(0, 25);
            this.returnedSizeLabel.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 712);
            this.Controls.Add(this.thumbiconPanel);
            this.Controls.Add(this.toolbarTableLayoutPanel);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Thumbico";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbiconPictureBox)).EndInit();
            this.thumbiconPanel.ResumeLayout(false);
            this.thumbiconPanel.PerformLayout();
            this.toolbarTableLayoutPanel.ResumeLayout(false);
            this.toolbarTableLayoutPanel.PerformLayout();
            this.desiredSizeFlowLayoutPanel.ResumeLayout(false);
            this.desiredSizeFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem openFileMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitFileMenuItem;
        private ToolStripMenuItem editMenuItem;
        private ToolStripMenuItem viewMenuItem;
        private ToolStripMenuItem helpMenuItem;
        private OpenFileDialog openFileDialog;
        private PictureBox thumbiconPictureBox;
        private Panel thumbiconPanel;
        private TableLayoutPanel toolbarTableLayoutPanel;
        private FlowLayoutPanel desiredSizeFlowLayoutPanel;
        private Label desiredSizeLabel;
        private NumericUpDown widthNumericUpDown;
        private Label xLabel;
        private NumericUpDown heightNumericUpDown;
        private Label returnedSizeLabel;
        private ToolStripMenuItem backgroundColorToolStripMenuItem;
        private ColorDialog colorDialog;
        private ToolStripMenuItem copyEditMenuItem;
        private ToolStripMenuItem imageMenuItem;
        private ToolStripMenuItem resizeToFitImageMenuItem;
        private ToolStripMenuItem biggerSizeOkImageMenuItem;
        private ToolStripMenuItem memoryOnlyImageMenuItem;
        private ToolStripMenuItem iconOnlyImageMenuItem;
        private ToolStripMenuItem thumbnailOnlyImageMenuItem;
        private ToolStripMenuItem inCacheOnlyImageMenuItem;
        private ToolStripMenuItem cropToSquareImageMenuItem;
        private ToolStripMenuItem wideThumbnailsImageMenuItem;
        private ToolStripMenuItem iconBackgroundImageMenuItem;
        private ToolStripMenuItem scaleUpImageMenuItem;
    }
}