namespace FileCountReporter
{
    /// <summary>
    /// File Count Reporter - A Windows Forms application that counts files and folders
    /// in a selected directory, with support for recursive subdirectory scanning.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Browse button click event.
        /// Opens a FolderBrowserDialog to allow the user to select a directory.
        /// Updates the textBoxFolderPath with the selected path and enables the Scan button.
        /// </summary>
        private void ButtonBrowse_Click(object? sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder to scan:";
                folderDialog.UseDescriptionForTitle = true;

                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    textBoxFolderPath.Text = folderDialog.SelectedPath;
                    buttonScan.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the Scan button click event.
        /// Validates the selected folder path and performs the file/folder count operation.
        /// Displays results in the respective output labels with error handling.
        /// </summary>
        private void ButtonScan_Click(object? sender, EventArgs e)
        {
            string folderPath = textBoxFolderPath.Text.Trim();

            // Validate that a path was selected
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageBox.Show("Please select a folder first.", "No Folder Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate that the path exists
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"The folder path does not exist:\n{folderPath}", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Count top-level files and folders
                int topLevelFiles = CountTopLevelFiles(folderPath);
                int topLevelFolders = CountTopLevelFolders(folderPath);

                // Count all files recursively (including all subfolders)
                int totalFilesRecursive = CountFilesRecursively(folderPath);

                // Display results in the UI
                labelTopLevelFilesValue.Text = topLevelFiles.ToString();
                labelTopLevelFoldersValue.Text = topLevelFolders.ToString();
                labelAllFilesRecursiveValue.Text = totalFilesRecursive.ToString();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied. You do not have permission to access this folder or some of its subfolders.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while scanning the folder:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetResults();
            }
        }

        /// <summary>
        /// Counts the number of files in the top-level directory only (non-recursive).
        /// </summary>
        /// <param name="folderPath">The directory path to scan</param>
        /// <returns>The count of top-level files</returns>
        private int CountTopLevelFiles(string folderPath)
        {
            try
            {
                return Directory.GetFiles(folderPath).Length;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }

        /// <summary>
        /// Counts the number of folders in the top-level directory only (non-recursive).
        /// </summary>
        /// <param name="folderPath">The directory path to scan</param>
        /// <returns>The count of top-level folders</returns>
        private int CountTopLevelFolders(string folderPath)
        {
            try
            {
                return Directory.GetDirectories(folderPath).Length;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }

        /// <summary>
        /// Counts all files in the directory and its subfolders recursively.
        /// This is the entry point for recursive counting and handles top-level exceptions.
        /// </summary>
        /// <param name="folderPath">The directory path to scan</param>
        /// <returns>The total count of files across all subfolders</returns>
        private int CountFilesRecursively(string folderPath)
        {
            try
            {
                return CountFilesRecursiveHelper(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }

        /// <summary>
        /// Helper method that performs the recursive traversal of directories and counts files.
        /// This method is separated from CountFilesRecursively to follow the single responsibility principle
        /// and allow for clear separation between entry point and recursive logic.
        /// </summary>
        /// <param name="folderPath">The current directory path being scanned</param>
        /// <returns>The count of files in the current directory and all its subfolders</returns>
        private int CountFilesRecursiveHelper(string folderPath)
        {
            int fileCount = 0;

            try
            {
                // Count files in the current directory
                fileCount += Directory.GetFiles(folderPath).Length;

                // Recursively count files in all subdirectories
                string[] subdirectories = Directory.GetDirectories(folderPath);

                foreach (string subdirectory in subdirectories)
                {
                    try
                    {
                        // Recursive call for each subdirectory
                        fileCount += CountFilesRecursiveHelper(subdirectory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Continue scanning other directories even if access is denied to one
                        continue;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }

            return fileCount;
        }

        /// <summary>
        /// Resets all result labels to zero.
        /// Called when an error occurs to ensure the UI doesn't display stale data.
        /// </summary>
        private void ResetResults()
        {
            labelTopLevelFilesValue.Text = "0";
            labelTopLevelFoldersValue.Text = "0";
            labelAllFilesRecursiveValue.Text = "0";
        }
    }
}
