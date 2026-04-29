# File Count Reporter

A professional Windows Forms application that analyzes directory structures and provides comprehensive file and folder statistics. Built with C# and .NET 10, this tool supports both shallow and recursive directory traversal with robust error handling.

## Features

- **Browse Functionality**: User-friendly folder selection via FolderBrowserDialog
- **Top-Level Statistics**: Count files and folders at the directory root level only
- **Recursive Analysis**: Count all files across the entire directory tree including subfolders
- **Error Handling**: Graceful handling of permission errors and invalid paths
- **Clean UI**: Professional Windows Forms interface with clear visual separation
- **Input Validation**: Comprehensive validation of user selections

## Project Structure

### Form1.cs (Main Logic)
The core implementation containing:
- **Event Handlers**: Button click handlers for Browse and Scan operations
- **File Counting Methods**: 
  - `CountTopLevelFiles()` - Counts files in the root directory only
  - `CountTopLevelFolders()` - Counts folders in the root directory only
  - `CountFilesRecursively()` - Entry point for recursive file counting
  - `CountFilesRecursiveHelper()` - Performs the actual recursive traversal
- **Utility Methods**:
  - `ResetResults()` - Clears output when errors occur

### Form1.Designer.cs (UI Layout)
The auto-generated designer file containing:
- Control declarations and initialization
- Event subscriptions
- Form layout and styling

## How It Works

### Version 1 (Top-Level Only)
```
Selected Folder: C:\Users\John\Documents
├─ Files (Top-Level Only): 5
├─ Folders (Top-Level Only): 3
```

### Version 2 (Recursive)
```
Selected Folder: C:\Users\John\Documents
├─ Files (Top-Level Only): 5
├─ Folders (Top-Level Only): 3
└─ Files (All Subfolders - Recursive): 47  ← Includes all nested files
```

## Event Handlers Explained

### ButtonBrowse_Click
**Purpose**: Opens a folder selection dialog

**Process**:
1. Creates a `FolderBrowserDialog` instance
2. Sets user-friendly dialog description
3. Checks if user selected a valid path
4. Updates the path textbox
5. Enables the Scan button

**Code Pattern**:
```csharp
using (var folderDialog = new FolderBrowserDialog())
{
    if (folderDialog.ShowDialog() == DialogResult.OK)
    {
        // Process selected path
    }
}
```

### ButtonScan_Click
**Purpose**: Initiates the file/folder counting process

**Process**:
1. Validates that a path is selected
2. Checks that the path exists
3. Calls counting methods within a try-catch block
4. Updates result labels with counts
5. Handles and displays errors gracefully

**Error Handling**:
- `UnauthorizedAccessException` - Permission denied errors
- General `Exception` - Any other unexpected errors
- Results are reset to zero if an error occurs

## Helper Methods Explained

### CountTopLevelFiles(string folderPath)
- **Purpose**: Count only the files in the root directory
- **Method Used**: `Directory.GetFiles(folderPath)`
- **Return**: Integer count
- **Complexity**: O(1) - Single directory read

### CountTopLevelFolders(string folderPath)
- **Purpose**: Count only the folders in the root directory
- **Method Used**: `Directory.GetDirectories(folderPath)`
- **Return**: Integer count
- **Complexity**: O(1) - Single directory read

### CountFilesRecursively(string folderPath)
- **Purpose**: Entry point that wraps recursive counting
- **Responsibility**: Handle top-level exceptions
- **Delegates To**: `CountFilesRecursiveHelper()`

### CountFilesRecursiveHelper(string folderPath)
- **Purpose**: Recursively traverse all subdirectories and count files
- **Recursive Pattern**:
  1. Count files in current directory with `Directory.GetFiles(folderPath)`
  2. Get all subdirectories with `Directory.GetDirectories(folderPath)`
  3. Call itself for each subdirectory
  4. Handle `UnauthorizedAccessException` per directory (continues scanning others)
  5. Return accumulated file count

**Example Traversal**:
```
C:\Documents
├─ file1.txt (counted)
├─ file2.pdf (counted)
├─ Folder1\
│  ├─ file3.doc (counted via recursive call)
│  └─ Folder2\
│     └─ file4.xlsx (counted via nested recursive call)
└─ Folder3\
   └─ file5.mp3 (counted via recursive call)

Total: 5 files across all levels
```

### ResetResults()
- **Purpose**: Clear all result labels
- **Called When**: An error occurs during scanning
- **Reason**: Prevents stale data from being displayed

## UI Controls

| Control | Type | Purpose |
|---------|------|---------|
| textBoxFolderPath | TextBox | Displays selected folder path (read-only) |
| buttonBrowse | Button | Opens FolderBrowserDialog |
| buttonScan | Button | Initiates file/folder counting |
| labelTopLevelFilesValue | Label | Shows count of top-level files |
| labelTopLevelFoldersValue | Label | Shows count of top-level folders |
| labelAllFilesRecursiveValue | Label | Shows count of all files recursively |

## Error Handling Strategy

The application uses a **catch-and-continue** approach for recursive scanning:

```csharp
foreach (string subdirectory in subdirectories)
{
    try
    {
        fileCount += CountFilesRecursiveHelper(subdirectory);
    }
    catch (UnauthorizedAccessException)
    {
        // Continue scanning other directories
        continue;
    }
}
```

This ensures that:
- Permission-denied folders don't stop the entire scan
- Users get the maximum possible data
- Errors are clearly reported in the UI

## Technical Requirements

- **Framework**: .NET 10
- **Platform**: Windows Forms
- **Language**: C# 12+
- **Minimum Requirements**:
  - Windows 7 or later
  - .NET 10 Runtime

## Building the Project

```powershell
dotnet build
```

## Running the Application

```powershell
dotnet run
```

Or:
```powershell
File Count Reporter.exe
```

## Code Quality Features

✅ **Comprehensive Comments**: Each method and event is clearly documented
✅ **Separation of Concerns**: Logic is split across focused methods
✅ **Exception Handling**: Robust error handling with user-friendly messages
✅ **Input Validation**: Checks for null, empty, and invalid paths
✅ **UI Responsiveness**: Synchronous operations on appropriate threads
✅ **Resource Management**: Proper disposal of dialogs via `using` statements
✅ **Recursion Safety**: Separate helper method for recursive logic

## Usage Example

1. Launch the application
2. Click "Browse" button
3. Select a folder (e.g., `C:\Users\YourName\Documents`)
4. Click "Scan" button
5. View results:
   - Total files at root level
   - Total folders at root level
   - All files including subfolders

## Future Enhancements

- Progress bar for large directory trees
- Folder size calculation
- File type filtering
- Export results to CSV
- Async scanning for responsiveness
- Exclude patterns (e.g., skip .git, node_modules)

## License

This project is available as-is for educational and personal use.

## Contributing

For improvements or bug fixes, please create a pull request with:
- Clear description of changes
- Comments explaining the logic
- Updated documentation if applicable

---

**Version**: 2.0  
**Last Updated**: 2024  
**Status**: Production Ready
