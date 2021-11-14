namespace Thumbico
{
    public partial class MainForm : Form
    {
        private string thumbiconFileName;
        private Size thumbiconSize = new(256, 256);
        private ThumbnailFlags thumbiconFlags;

        public MainForm()
        {
            InitializeComponent();
        }

        private string ThumbiconFileName
        {
            get
            {
                return this.thumbiconFileName;
            }

            set
            {
                this.thumbiconFileName = value;
                this.ReloadThumbicon();
            }
        }

        private Size ThumbiconSize
        {
            get
            {
                return this.thumbiconSize;
            }

            set
            {
                this.thumbiconSize = value;
                // this.desiredSizeStatusBarPanel.Text = $"Desired size: {this.thumbiconSize.Width} x {this.thumbiconSize.Height}";
                this.ReloadThumbicon();
            }
        }

        private ThumbnailFlags ThumbiconFlags
        {
            get
            {
                return this.thumbiconFlags;
            }

            set
            {
                this.thumbiconFlags = value;
                this.ReloadThumbicon();
            }
        }

        /// <summary>
        /// Reloads the thumbnail or icon of the current file
        /// </summary>
        private void ReloadThumbicon()
        {
            Image oldThumbnail = this.thumbiconPictureBox.Image;

            // Try to get the thumbnail or icon from the Windows Shell
            try
            {
                this.thumbiconPictureBox.Image = ShellThumbnail.GetThumbnail(
                    this.ThumbiconFileName,
                    this.ThumbiconSize.Width,
                    this.ThumbiconSize.Height,
                    this.ThumbiconFlags,
                    out bool isIcon);

                this.UpdateImagePosition();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error loading thumbnail or icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool haveImage = this.thumbiconPictureBox.Image != null;
            // this.actualSizeStatusBarPanel.Text = haveImage ? $"Actual size: {this.thumbiconPictureBox.Image.Width} x {this.thumbiconPictureBox.Image.Height}" : string.Empty;

            // Because thumbnail bitmaps can be large, release the memory used by previous thumbnail managed bitmap
            if (oldThumbnail != null)
            {
                oldThumbnail.Dispose();
                GC.Collect();
            }
        }

        private void UpdateImagePosition()
        {
            if (this.thumbiconPictureBox.Image != null)
            {
                this.thumbiconPanel.Visible = false;
                try
                {
                    Point oldPosition = this.thumbiconPanel.AutoScrollPosition;
                    this.thumbiconPanel.AutoScrollPosition = new Point(0, 0);
                    this.thumbiconPictureBox.Left = Math.Max((this.thumbiconPanel.ClientSize.Width - this.thumbiconPictureBox.Image.Width) / 2, 0);
                    this.thumbiconPictureBox.Top = Math.Max((this.thumbiconPanel.ClientSize.Height - this.thumbiconPictureBox.Image.Height) / 2, 0);
                    oldPosition.X = Math.Abs(oldPosition.X);
                    oldPosition.Y = Math.Abs(oldPosition.Y);
                    this.thumbiconPanel.AutoScrollPosition = oldPosition;
                }
                finally
                {
                    this.thumbiconPanel.Visible = true;
                }
            }
        }


        // *************************************
        // File Menu Event Handlers
        // *************************************

        /// <summary>
        /// Opens the Open File common dialog box and loads the thumbicon of the selected file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.ThumbiconFileName = this.openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Closes the main form (and the app).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // *************************************
        // Form Event Handlers
        // *************************************

        /// <summary>
        /// Update the thumbicon image position when the form is resized.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            this.UpdateImagePosition();
        }

        /// <summary>
        /// Informs the system that the main form accepts drag and drop operations with file/folder items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        /// <summary>
        /// Gets the file/folder that was dragged and dropped on the main form and loads its thumbicon.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileItems = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileItems.Length > 0)
            {
                this.ThumbiconFileName = fileItems[0];
            }
        }
    }
}