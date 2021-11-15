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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.fullScreenPreviewViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.thumbiconPictureBox = new System.Windows.Forms.PictureBox();
            this.thumbiconPanel = new System.Windows.Forms.Panel();
            this.toolbarTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.sizeFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.customSizeFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.xLabel = new System.Windows.Forms.Label();
            this.heightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.sizeApplyButton = new System.Windows.Forms.Button();
            this.returnedSizeLabel = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbiconPictureBox)).BeginInit();
            this.thumbiconPanel.SuspendLayout();
            this.toolbarTableLayoutPanel.SuspendLayout();
            this.sizeFlowLayoutPanel.SuspendLayout();
            this.customSizeFlowLayoutPanel.SuspendLayout();
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
            this.toolStripSeparator2,
            this.saveFileMenuItem,
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
            this.openFileMenuItem.Size = new System.Drawing.Size(304, 34);
            this.openFileMenuItem.Text = "&Open...";
            this.openFileMenuItem.Click += new System.EventHandler(this.OpenFileMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(301, 6);
            // 
            // saveFileMenuItem
            // 
            this.saveFileMenuItem.Name = "saveFileMenuItem";
            this.saveFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveFileMenuItem.Size = new System.Drawing.Size(304, 34);
            this.saveFileMenuItem.Text = "&Save Image As...";
            this.saveFileMenuItem.Click += new System.EventHandler(this.SaveFileMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(301, 6);
            // 
            // exitFileMenuItem
            // 
            this.exitFileMenuItem.Name = "exitFileMenuItem";
            this.exitFileMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitFileMenuItem.Size = new System.Drawing.Size(304, 34);
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
            this.copyEditMenuItem.Size = new System.Drawing.Size(273, 34);
            this.copyEditMenuItem.Text = "&Copy Image";
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
            this.fullScreenPreviewViewMenuItem,
            this.backgroundColorViewMenuItem});
            this.viewMenuItem.Name = "viewMenuItem";
            this.viewMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewMenuItem.Text = "&View";
            // 
            // fullScreenPreviewViewMenuItem
            // 
            this.fullScreenPreviewViewMenuItem.Name = "fullScreenPreviewViewMenuItem";
            this.fullScreenPreviewViewMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.fullScreenPreviewViewMenuItem.Size = new System.Drawing.Size(334, 34);
            this.fullScreenPreviewViewMenuItem.Text = "Toggle Preview Mode";
            this.fullScreenPreviewViewMenuItem.Click += new System.EventHandler(this.TogglePreviewModeViewMenuItem_Click);
            // 
            // backgroundColorViewMenuItem
            // 
            this.backgroundColorViewMenuItem.Name = "backgroundColorViewMenuItem";
            this.backgroundColorViewMenuItem.Size = new System.Drawing.Size(334, 34);
            this.backgroundColorViewMenuItem.Text = "Preview &Background Color...";
            this.backgroundColorViewMenuItem.Click += new System.EventHandler(this.BackgroundColorViewMenuItem_Click);
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
            this.thumbiconPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.thumbiconPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.thumbiconPictureBox.Location = new System.Drawing.Point(373, 183);
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
            this.thumbiconPanel.Location = new System.Drawing.Point(0, 90);
            this.thumbiconPanel.Name = "thumbiconPanel";
            this.thumbiconPanel.Size = new System.Drawing.Size(1002, 622);
            this.thumbiconPanel.TabIndex = 2;
            // 
            // toolbarTableLayoutPanel
            // 
            this.toolbarTableLayoutPanel.AutoSize = true;
            this.toolbarTableLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolbarTableLayoutPanel.ColumnCount = 2;
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.toolbarTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.toolbarTableLayoutPanel.Controls.Add(this.sizeFlowLayoutPanel, 0, 0);
            this.toolbarTableLayoutPanel.Controls.Add(this.returnedSizeLabel, 1, 0);
            this.toolbarTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbarTableLayoutPanel.Location = new System.Drawing.Point(0, 33);
            this.toolbarTableLayoutPanel.Name = "toolbarTableLayoutPanel";
            this.toolbarTableLayoutPanel.Padding = new System.Windows.Forms.Padding(8);
            this.toolbarTableLayoutPanel.RowCount = 1;
            this.toolbarTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolbarTableLayoutPanel.Size = new System.Drawing.Size(1002, 57);
            this.toolbarTableLayoutPanel.TabIndex = 3;
            // 
            // sizeFlowLayoutPanel
            // 
            this.sizeFlowLayoutPanel.AutoSize = true;
            this.sizeFlowLayoutPanel.Controls.Add(this.sizeComboBox);
            this.sizeFlowLayoutPanel.Controls.Add(this.customSizeFlowLayoutPanel);
            this.sizeFlowLayoutPanel.Location = new System.Drawing.Point(11, 11);
            this.sizeFlowLayoutPanel.MinimumSize = new System.Drawing.Size(0, 35);
            this.sizeFlowLayoutPanel.Name = "sizeFlowLayoutPanel";
            this.sizeFlowLayoutPanel.Size = new System.Drawing.Size(461, 35);
            this.sizeFlowLayoutPanel.TabIndex = 3;
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Items.AddRange(new object[] {
            "Custom size",
            "16 x 16",
            "24 x 24",
            "32 x 32",
            "48 x 48",
            "64 x 64",
            "96 x 96",
            "128 x 128",
            "192 x 192",
            "256 x 256",
            "512 x 512",
            "1024 x 1024"});
            this.sizeComboBox.Location = new System.Drawing.Point(0, 1);
            this.sizeComboBox.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(182, 33);
            this.sizeComboBox.TabIndex = 6;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.SizeComboBox_SelectedIndexChanged);
            // 
            // customSizeFlowLayoutPanel
            // 
            this.customSizeFlowLayoutPanel.AutoSize = true;
            this.customSizeFlowLayoutPanel.Controls.Add(this.widthNumericUpDown);
            this.customSizeFlowLayoutPanel.Controls.Add(this.xLabel);
            this.customSizeFlowLayoutPanel.Controls.Add(this.heightNumericUpDown);
            this.customSizeFlowLayoutPanel.Controls.Add(this.sizeApplyButton);
            this.customSizeFlowLayoutPanel.Location = new System.Drawing.Point(188, 0);
            this.customSizeFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.customSizeFlowLayoutPanel.Name = "customSizeFlowLayoutPanel";
            this.customSizeFlowLayoutPanel.Size = new System.Drawing.Size(273, 35);
            this.customSizeFlowLayoutPanel.TabIndex = 7;
            // 
            // widthNumericUpDown
            // 
            this.widthNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.widthNumericUpDown.AutoSize = true;
            this.widthNumericUpDown.Location = new System.Drawing.Point(0, 2);
            this.widthNumericUpDown.Margin = new System.Windows.Forms.Padding(0);
            this.widthNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.widthNumericUpDown.Name = "widthNumericUpDown";
            this.widthNumericUpDown.Size = new System.Drawing.Size(86, 31);
            this.widthNumericUpDown.TabIndex = 1;
            this.widthNumericUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // xLabel
            // 
            this.xLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(89, 5);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(20, 25);
            this.xLabel.TabIndex = 3;
            this.xLabel.Text = "x";
            this.xLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // heightNumericUpDown
            // 
            this.heightNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.heightNumericUpDown.AutoSize = true;
            this.heightNumericUpDown.Location = new System.Drawing.Point(112, 2);
            this.heightNumericUpDown.Margin = new System.Windows.Forms.Padding(0);
            this.heightNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.heightNumericUpDown.Name = "heightNumericUpDown";
            this.heightNumericUpDown.Size = new System.Drawing.Size(86, 31);
            this.heightNumericUpDown.TabIndex = 4;
            this.heightNumericUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // sizeApplyButton
            // 
            this.sizeApplyButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sizeApplyButton.AutoSize = true;
            this.sizeApplyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sizeApplyButton.Location = new System.Drawing.Point(204, 0);
            this.sizeApplyButton.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.sizeApplyButton.Name = "sizeApplyButton";
            this.sizeApplyButton.Size = new System.Drawing.Size(69, 35);
            this.sizeApplyButton.TabIndex = 5;
            this.sizeApplyButton.Text = "&Apply";
            this.sizeApplyButton.UseVisualStyleBackColor = true;
            this.sizeApplyButton.Click += new System.EventHandler(this.SizeApplyButton_Click);
            // 
            // returnedSizeLabel
            // 
            this.returnedSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.returnedSizeLabel.AutoSize = true;
            this.returnedSizeLabel.Location = new System.Drawing.Point(991, 16);
            this.returnedSizeLabel.Name = "returnedSizeLabel";
            this.returnedSizeLabel.Size = new System.Drawing.Size(0, 25);
            this.returnedSizeLabel.TabIndex = 4;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
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
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.Text = "Thumbico";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbiconPictureBox)).EndInit();
            this.thumbiconPanel.ResumeLayout(false);
            this.thumbiconPanel.PerformLayout();
            this.toolbarTableLayoutPanel.ResumeLayout(false);
            this.toolbarTableLayoutPanel.PerformLayout();
            this.sizeFlowLayoutPanel.ResumeLayout(false);
            this.sizeFlowLayoutPanel.PerformLayout();
            this.customSizeFlowLayoutPanel.ResumeLayout(false);
            this.customSizeFlowLayoutPanel.PerformLayout();
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
        private FlowLayoutPanel sizeFlowLayoutPanel;
        private NumericUpDown widthNumericUpDown;
        private Label xLabel;
        private NumericUpDown heightNumericUpDown;
        private Label returnedSizeLabel;
        private ToolStripMenuItem backgroundColorViewMenuItem;
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
        private Button sizeApplyButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem saveFileMenuItem;
        private SaveFileDialog saveFileDialog;
        private ComboBox sizeComboBox;
        private FlowLayoutPanel customSizeFlowLayoutPanel;
        private ToolStripMenuItem fullScreenPreviewViewMenuItem;
    }
}