using System.Drawing.Imaging;
using Thumbico.Settings;

namespace Thumbico
{
    public partial class MainForm : Form
    {
        private readonly AppSettings appSettings;

        private string thumbiconFileName;
        private Size thumbiconSize = new(256, 256);
        private ThumbnailFlags thumbiconFlags;

        public MainForm()
        {
            InitializeComponent();

            InitFlagsImageMenuItems();

            // Load app settings
            appSettings = AppSettings.Load();
        }

        private string ThumbiconFileName
        {
            get
            {
                return thumbiconFileName;
            }

            set
            {
                thumbiconFileName = value;
                Text = $"{thumbiconFileName} - Thumbico";
                ReloadThumbicon();
            }
        }

        private Size ThumbiconSize
        {
            get
            {
                return thumbiconSize;
            }

            set
            {
                thumbiconSize = value;
                ReloadThumbicon();
            }
        }

        private ThumbnailFlags ThumbiconFlags
        {
            get
            {
                return thumbiconFlags;
            }

            set
            {
                thumbiconFlags = value;
                ReloadThumbicon();
            }
        }

        /// <summary>
        /// Reloads the thumbnail or icon of the current file
        /// </summary>
        private void ReloadThumbicon()
        {
            Image oldThumbnail = thumbiconPictureBox.Image;

            // Try to get the thumbnail or icon from the Windows Shell
            try
            {
                thumbiconPictureBox.Image = ShellThumbnail.GetThumbnail(
                    ThumbiconFileName,
                    ThumbiconSize.Width,
                    ThumbiconSize.Height,
                    ThumbiconFlags,
                    out bool isIcon);

                UpdateImagePosition();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error loading thumbnail or icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool haveImage = thumbiconPictureBox.Image != null;
            returnedSizeLabel.Text = haveImage ? $"Returned size: {thumbiconPictureBox.Image.Width} x {thumbiconPictureBox.Image.Height}" : string.Empty;

            // Because thumbnail bitmaps can be large, release the memory used by previous thumbnail managed bitmap
            if (oldThumbnail != null)
            {
                oldThumbnail.Dispose();
                GC.Collect();
            }
        }

        private void UpdateImagePosition()
        {
            if (thumbiconPictureBox.Image != null)
            {
                thumbiconPanel.Visible = false;
                try
                {
                    Point oldPosition = thumbiconPanel.AutoScrollPosition;
                    thumbiconPanel.AutoScrollPosition = new Point(0, 0);
                    thumbiconPictureBox.Left = Math.Max((thumbiconPanel.ClientSize.Width - thumbiconPictureBox.Image.Width) / 2, 0);
                    thumbiconPictureBox.Top = Math.Max((thumbiconPanel.ClientSize.Height - thumbiconPictureBox.Image.Height) / 2, 0);
                    oldPosition.X = Math.Abs(oldPosition.X);
                    oldPosition.Y = Math.Abs(oldPosition.Y);
                    thumbiconPanel.AutoScrollPosition = oldPosition;
                }
                finally
                {
                    thumbiconPanel.Visible = true;
                }
            }
        }

        private void InitFlagsImageMenuItems()
        {
            resizeToFitImageMenuItem.Tag = ThumbnailFlags.ResizeToFit;
            biggerSizeOkImageMenuItem.Tag = ThumbnailFlags.BiggerSizeOk;
            memoryOnlyImageMenuItem.Tag = ThumbnailFlags.MemoryOnly;
            iconOnlyImageMenuItem.Tag = ThumbnailFlags.IconOnly;
            thumbnailOnlyImageMenuItem.Tag = ThumbnailFlags.ThumbnailOnly;
            inCacheOnlyImageMenuItem.Tag = ThumbnailFlags.InCacheOnly;
            cropToSquareImageMenuItem.Tag = ThumbnailFlags.CropToSquare;
            wideThumbnailsImageMenuItem.Tag = ThumbnailFlags.WideThumbnails;
            iconBackgroundImageMenuItem.Tag = ThumbnailFlags.IconBackground;
            scaleUpImageMenuItem.Tag = ThumbnailFlags.ScaleUp;
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
            UpdateImagePosition();
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
                ThumbiconFileName = fileItems[0];
            }
        }

        /// <summary>
        /// Save the app settings when the main form is closing.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            appSettings.Save();
        }

        // *************************************
        // Toolbar Event Handlers
        // *************************************

        /// <summary>
        /// Applies a new custom or predefined size by reloading a thumbicon of that size.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void SizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            customSizeFlowLayoutPanel.Visible = sizeComboBox.SelectedIndex == 0;

            if (sizeComboBox.SelectedIndex == 0)
            {
                sizeApplyButton.PerformClick();
            }
            else
            {
                IEnumerable<int> size = sizeComboBox.Text.Split('x').Select(s => int.Parse(s.Trim()));
                ThumbiconSize = new Size(size.First(), size.Last());
            }
        }

        /// <summary>
        /// Applies a new custom size by reloading a thumbicon of that size.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void SizeApplyButton_Click(object sender, EventArgs e)
        {
            ThumbiconSize = new Size((int)widthNumericUpDown.Value, (int)heightNumericUpDown.Value);
        }

        // *************************************
        // File Menu Event Handlers
        // *************************************

        /// <summary>
        /// Opens the Open File common dialog box and loads the thumbicon of the selected file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OpenFileMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ThumbiconFileName = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Opens the Save File common dialog box and saves the thumbicon to a BMP, GIF, JPG or PNG file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void SaveFileMenuItem_Click(object sender, EventArgs e)
        {
            if (thumbiconPictureBox.Image != null)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat[] formats = { ImageFormat.Bmp, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png };
                    thumbiconPictureBox.Image.Save(
                        saveFileDialog.FileName,
                        formats[saveFileDialog.FilterIndex - 1]);
                }
            }
        }

        /// <summary>
        /// Closes the main form (and the app).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void ExitFileMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // *************************************
        // Edit Menu Event Handlers
        // *************************************

        /// <summary>
        /// Copies the current thumbicon image to clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void CopyEditMenuItem_Click(object sender, EventArgs e)
        {
            if (thumbiconPictureBox.Image != null)
            {
                Clipboard.SetImage(thumbiconPictureBox.Image);
            }
        }

        // *************************************
        // Image Menu Event Handlers
        // *************************************

        private void FlagsImageMenuItems_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem senderMenuItem)
            {
                senderMenuItem.Checked = !senderMenuItem.Checked;
                ThumbnailFlags flag = (ThumbnailFlags)senderMenuItem.Tag;
                ThumbiconFlags = senderMenuItem.Checked ? ThumbiconFlags | flag : ThumbiconFlags & ~flag;
            }
        }

        // *************************************
        // View Menu Event Handlers
        // *************************************

        /// <summary>
        /// Toggles the preview mode.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void TogglePreviewModeViewMenuItem_Click(object sender, EventArgs e)
        {
            TogglePreviewMode(mainMenuStrip.Visible);
        }

        /// <summary>
        /// Opens the Color common dialog box and lets the user change the preview background color.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void BackgroundColorViewMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog.Color = appSettings.PreviewBackColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                appSettings.PreviewBackColor = colorDialog.Color;
            }
        }

        private void TogglePreviewMode(bool preview)
        {
            thumbiconPictureBox.BackColor = preview ? appSettings.PreviewBackColor : SystemColors.Window;
            thumbiconPanel.BackColor = preview ? appSettings.PreviewBackColor : SystemColors.Control;
            thumbiconPictureBox.BorderStyle = preview ? BorderStyle.None : BorderStyle.FixedSingle;
            toolbarTableLayoutPanel.Visible = mainMenuStrip.Visible = !preview;
            UpdateImagePosition();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!mainMenuStrip.Visible) TogglePreviewMode(false);
            }
        }
    }
}