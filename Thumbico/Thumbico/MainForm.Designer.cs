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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1002, 33);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(235, 34);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(235, 34);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(58, 29);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // thumbiconPictureBox
            // 
            this.thumbiconPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.thumbiconPictureBox.Location = new System.Drawing.Point(272, 80);
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
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
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
    }
}