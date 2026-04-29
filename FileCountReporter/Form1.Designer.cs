namespace FileCountReporter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // UI Controls
        private TextBox textBoxFolderPath = null!;
        private Button buttonBrowse = null!;
        private Button buttonScan = null!;
        private Label labelFolderPath = null!;
        private Label labelTopLevelFiles = null!;
        private Label labelTopLevelFolders = null!;
        private Label labelAllFilesRecursive = null!;
        private Label labelTopLevelFilesValue = null!;
        private Label labelTopLevelFoldersValue = null!;
        private Label labelAllFilesRecursiveValue = null!;

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
            components = new System.ComponentModel.Container();

            // Form settings
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 350);
            Text = "File Count Reporter";
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            // Label: Folder Path
            labelFolderPath = new Label
            {
                Text = "Folder Path:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                AutoSize = true
            };
            Controls.Add(labelFolderPath);

            // TextBox: Folder Path
            textBoxFolderPath = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(500, 35),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(textBoxFolderPath);

            // Button: Browse
            buttonBrowse = new Button
            {
                Text = "Browse",
                Location = new Point(530, 45),
                Size = new Size(80, 35),
                BackColor = SystemColors.Control,
                Cursor = Cursors.Hand
            };
            buttonBrowse.Click += ButtonBrowse_Click;
            Controls.Add(buttonBrowse);

            // Button: Scan
            buttonScan = new Button
            {
                Text = "Scan",
                Location = new Point(620, 45),
                Size = new Size(60, 35),
                BackColor = SystemColors.Control,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            buttonScan.Click += ButtonScan_Click;
            Controls.Add(buttonScan);

            // Separator line (visual grouping)
            var separatorPanel = new Panel
            {
                BackColor = Color.LightGray,
                Location = new Point(20, 90),
                Size = new Size(660, 1)
            };
            Controls.Add(separatorPanel);

            // Label: Top-Level Files
            labelTopLevelFiles = new Label
            {
                Text = "Files (Top-Level Only):",
                Location = new Point(20, 110),
                Size = new Size(200, 25),
                AutoSize = true
            };
            Controls.Add(labelTopLevelFiles);

            labelTopLevelFilesValue = new Label
            {
                Text = "0",
                Location = new Point(250, 110),
                Size = new Size(50, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            Controls.Add(labelTopLevelFilesValue);

            // Label: Top-Level Folders
            labelTopLevelFolders = new Label
            {
                Text = "Folders (Top-Level Only):",
                Location = new Point(20, 145),
                Size = new Size(200, 25),
                AutoSize = true
            };
            Controls.Add(labelTopLevelFolders);

            labelTopLevelFoldersValue = new Label
            {
                Text = "0",
                Location = new Point(250, 145),
                Size = new Size(50, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            Controls.Add(labelTopLevelFoldersValue);

            // Label: All Files (Recursive)
            labelAllFilesRecursive = new Label
            {
                Text = "Files (All Subfolders - Recursive):",
                Location = new Point(20, 180),
                Size = new Size(250, 25),
                AutoSize = true
            };
            Controls.Add(labelAllFilesRecursive);

            labelAllFilesRecursiveValue = new Label
            {
                Text = "0",
                Location = new Point(250, 180),
                Size = new Size(50, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            Controls.Add(labelAllFilesRecursiveValue);
        }

        #endregion
    }
}
