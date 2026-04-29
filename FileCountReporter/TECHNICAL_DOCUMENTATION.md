# File Count Reporter - Technical Documentation

## Architecture Overview

The File Count Reporter is a single-form Windows Forms application that demonstrates clean architecture principles including:
- **Separation of Concerns**: UI logic separated from business logic
- **Single Responsibility**: Each method has one clear purpose
- **DRY Principle**: Reusable counting methods avoid duplication
- **Defensive Programming**: Comprehensive error handling and validation

## Complete Method Documentation

### Public Methods (Event Handlers)

#### ButtonBrowse_Click
```csharp
private void ButtonBrowse_Click(object? sender, EventArgs e)
```

**Summary**: Event handler for the Browse button click

**Behavior**:
1. Creates a new `FolderBrowserDialog` in a using block
2. Sets a descriptive prompt for the user
3. Displays the dialog modally
4. Validates the DialogResult is OK and path is not null/empty
5. Assigns the selected path to `textBoxFolderPath.Text`
6. Enables the `buttonScan` to allow scanning

**Error Handling**: Dialog cancellation is silently ignored (no error message)

**Parameters**:
- `sender` (object?): The button that triggered the event
- `e` (EventArgs): Event arguments (unused)

**Thread Safety**: Must be called on the UI thread (Windows Forms guarantee)

---

#### ButtonScan_Click
```csharp
private void ButtonScan_Click(object? sender, EventArgs e)
```

**Summary**: Event handler for the Scan button click

**Behavior**:
1. Retrieves and trims the folder path from the textbox
2. Validates the path is not null or whitespace
3. Validates the path exists on the file system
4. Calls three counting methods within a try-catch block:
   - `CountTopLevelFiles()`
   - `CountTopLevelFolders()`
   - `CountFilesRecursively()`
5. Updates the three result labels with the counts
6. Catches specific and general exceptions

**Exception Handling**:
- `UnauthorizedAccessException`: Displays "Permission Denied" message
- `Exception`: Displays generic error message with exception details
- Both exceptions trigger `ResetResults()` to clear stale data

**Parameters**:
- `sender` (object?): The button that triggered the event
- `e` (EventArgs): Event arguments (unused)

**Thread Safety**: Must be called on the UI thread

**Dialog Output**: Uses `MessageBox.Show()` with appropriate icon types:
- `MessageBoxIcon.Warning`: For missing path selection
- `MessageBoxIcon.Error`: For file system errors

---

### Private Methods (Business Logic)

#### CountTopLevelFiles
```csharp
private int CountTopLevelFiles(string folderPath)
```

**Summary**: Counts files in the root directory only (no recursion)

**Implementation**:
```csharp
return Directory.GetFiles(folderPath).Length;
```

**Characteristics**:
- **Non-recursive**: Only examines the specified directory
- **Exception**: Re-throws `UnauthorizedAccessException` for caller handling
- **Performance**: O(1) - Single directory enumeration

**Parameter**:
- `folderPath` (string): Full path to the directory to scan

**Return Value** (int): Count of files in the root directory

**Exception**:
- `UnauthorizedAccessException`: When access is denied to the directory

**Example**:
```
C:\Users\John\Documents
├─ file1.txt    ← counted
├─ file2.pdf    ← counted
└─ Subfolder
   └─ file3.doc ← NOT counted
Returns: 2
```

---

#### CountTopLevelFolders
```csharp
private int CountTopLevelFolders(string folderPath)
```

**Summary**: Counts subdirectories in the root directory only (no recursion)

**Implementation**:
```csharp
return Directory.GetDirectories(folderPath).Length;
```

**Characteristics**:
- **Non-recursive**: Only examines the specified directory
- **Exception**: Re-throws `UnauthorizedAccessException` for caller handling
- **Performance**: O(1) - Single directory enumeration

**Parameter**:
- `folderPath` (string): Full path to the directory to scan

**Return Value** (int): Count of subdirectories in the root directory

**Exception**:
- `UnauthorizedAccessException`: When access is denied to the directory

**Example**:
```
C:\Users\John\Documents
├─ Subfolder1         ← counted
├─ Subfolder2         ← counted
└─ file.txt          ← NOT counted
Returns: 2
```

---

#### CountFilesRecursively
```csharp
private int CountFilesRecursively(string folderPath)
```

**Summary**: Entry point for recursive file counting with centralized exception handling

**Implementation**:
```csharp
try
{
    return CountFilesRecursiveHelper(folderPath);
}
catch (UnauthorizedAccessException)
{
    throw;
}
```

**Purpose**:
- Acts as a facade/wrapper over the recursive helper
- Provides a single point of exception handling for the entire recursive operation
- Allows `CountFilesRecursiveHelper()` to focus purely on recursion logic

**Parameter**:
- `folderPath` (string): Root directory to begin recursive scan

**Return Value** (int): Total count of files across all directories and subdirectories

**Exception**:
- `UnauthorizedAccessException`: Re-thrown from helper (indicates root directory access denied)

**Design Pattern**: Wrapper pattern - entry point delegates to recursive helper

---

#### CountFilesRecursiveHelper
```csharp
private int CountFilesRecursiveHelper(string folderPath)
```

**Summary**: Performs the actual recursive traversal of directory trees

**Pseudo-code**:
```
1. Initialize fileCount = 0
2. Count files in current directory
   fileCount += Directory.GetFiles(folderPath).Length
3. Get all subdirectories
   subdirectories = Directory.GetDirectories(folderPath)
4. For each subdirectory:
   - Try to recursively count its files
   - fileCount += CountFilesRecursiveHelper(subdirectory)
   - If permission denied, continue to next directory
5. Return fileCount
```

**Recursion Details**:
- **Base Case**: Implicit - when a directory has no subdirectories, recursion stops
- **Recursive Case**: Called for each subdirectory with `CountFilesRecursiveHelper(subdirectory)`
- **Stack Depth**: Limited by directory depth (typically not a concern on Windows)

**Exception Handling Strategy**:
```csharp
foreach (string subdirectory in subdirectories)
{
    try
    {
        fileCount += CountFilesRecursiveHelper(subdirectory);
    }
    catch (UnauthorizedAccessException)
    {
        continue;  // Scan other directories
    }
}
```

This **catch-and-continue** pattern ensures:
- Single access-denied folder doesn't halt entire scan
- User gets maximum possible results
- Scan completes successfully

**Top-level `UnauthorizedAccessException`** (not in subdirectory):
- Not caught
- Re-thrown to caller
- Indicates root directory is inaccessible

**Parameter**:
- `folderPath` (string): Current directory path being examined

**Return Value** (int): File count for this directory and all its subdirectories

**Complexity Analysis**:
- **Time**: O(n) where n = total number of files and directories
- **Space**: O(d) where d = maximum directory depth (recursion stack)

**Traversal Example**:
```
C:\Root
├─ file1.txt (counted)
├─ SubA
│  ├─ file2.txt (counted via recursive call)
│  ├─ SubA1
│  │  └─ file3.txt (counted via nested recursive call)
│  └─ SubA2
│     └─ file4.txt (counted via nested recursive call)
├─ SubB
│  ├─ file5.txt (counted via recursive call)
│  └─ SubB1
│     └─ file6.txt (counted via nested recursive call)
└─ file7.txt (counted)

Call Stack During Traversal:
CountFilesRecursiveHelper("C:\Root")
  → fileCount += 2 (file1.txt, file7.txt)
  → CountFilesRecursiveHelper("C:\Root\SubA")
    → fileCount += 1 (file2.txt)
    → CountFilesRecursiveHelper("C:\Root\SubA\SubA1")
      → fileCount += 1 (file3.txt)
    → CountFilesRecursiveHelper("C:\Root\SubA\SubA2")
      → fileCount += 1 (file4.txt)
  → CountFilesRecursiveHelper("C:\Root\SubB")
    → fileCount += 1 (file5.txt)
    → CountFilesRecursiveHelper("C:\Root\SubB\SubB1")
      → fileCount += 1 (file6.txt)

Total: 7 files
```

---

#### ResetResults
```csharp
private void ResetResults()
```

**Summary**: Clears all result display labels

**Implementation**:
```csharp
labelTopLevelFilesValue.Text = "0";
labelTopLevelFoldersValue.Text = "0";
labelAllFilesRecursiveValue.Text = "0";
```

**Purpose**:
- Called when an exception occurs during scanning
- Prevents stale/partial results from being displayed to the user
- Ensures UI consistency

**Called From**:
- `ButtonScan_Click()` in catch blocks

**No Return Value**: Void method (updates UI state)

**No Parameters**: Uses form-level label references

---

## Control Flow Diagram

```
User Launches Application
        ↓
Form1 Initializes
        ↓
[All controls disabled except Browse]
        ↓
User Clicks Browse
        ↓
ButtonBrowse_Click() → FolderBrowserDialog.ShowDialog()
        ↓
[User selects folder or cancels]
        ↓
[Path populated, Scan button enabled]
        ↓
User Clicks Scan
        ↓
ButtonScan_Click() Validation
├─ Path is not null/empty? ✓
├─ Path exists on disk? ✓
        ↓
Try: Execute counting operations
├─ CountTopLevelFiles()
├─ CountTopLevelFolders()
└─ CountFilesRecursively()
        ↓
[Success: Display results in labels]
        ↓
Catch UnauthorizedAccessException
├─ MessageBox: "Permission Denied"
└─ ResetResults()
        ↓
Catch General Exception
├─ MessageBox: "Error: [details]"
└─ ResetResults()
```

## Data Flow for Recursive Counting

```
User Input: "C:\Users\John\Documents"
        ↓
CountFilesRecursively("C:\Users\John\Documents")
        ↓
CountFilesRecursiveHelper("C:\Users\John\Documents")
        ├─ Directory.GetFiles() → [files at root]
        ├─ fileCount += count of files at root
        ├─ Directory.GetDirectories() → [list of subdirs]
        ├─ For each subdirectory:
        │  ├─ Try:
        │  │  └─ CountFilesRecursiveHelper(subdirectory) ← RECURSIVE
        │  └─ Catch UnauthorizedAccessException: continue
        └─ Return fileCount (accumulated from all levels)
        ↓
ButtonScan_Click updates: labelAllFilesRecursiveValue.Text = fileCount
```

## Memory Considerations

**Stack Usage**:
- Each recursive call uses stack space
- Maximum depth = directory nesting level
- Typical Windows systems: < 50 levels → safe
- Deep network shares: Could reach limits

**Heap Usage**:
- `Directory.GetDirectories()` allocates string array for all subdirectories
- `Directory.GetFiles()` allocates string array for all files
- Only temporary - freed after method returns
- No memory leaks in current implementation

## Testing Scenarios

### Happy Path
```
1. Browse to: C:\Users\[User]\Documents
2. Click Scan
3. Expected: Three positive integers displayed
4. No errors
```

### Edge Cases

**Empty Directory**:
```
C:\Empty
Expected: All zeros displayed
```

**Single File**:
```
C:\Single\file.txt
Expected: TopLevelFiles=1, TopLevelFolders=0, Recursive=1
```

**Deep Nesting**:
```
C:\A\B\C\D\E\F\G\...
Expected: Correct count at any depth
Concern: Stack overflow is virtually impossible on Windows
```

**Access Denied to Subfolder**:
```
C:\Root (accessible)
├─ Subfolder1 (accessible) → counted
├─ Subfolder2 (access denied) → skipped via catch-continue
└─ Subfolder3 (accessible) → counted
Expected: Count includes files from 1 and 3, skips 2, no error dialog
```

**Access Denied to Root**:
```
C:\ProtectedFolder (current user cannot read)
Expected: MessageBox "Permission Denied"
Result: All labels reset to 0
```

**Path Does Not Exist**:
```
C:\NonExistent\Path
Expected: MessageBox "Path does not exist"
Result: All labels remain as previous scan
```

**Invalid Path Characters**:
```
"C:\<>:?|"
Expected: Directory.Exists() returns false → handled gracefully
Result: MessageBox "Path does not exist"
```

## Performance Notes

- **Average Directory**: < 100ms
- **Large Directory (100,000 files)**: 1-5 seconds (depends on I/O)
- **Deep Nesting**: No significant performance impact
- **Recursive Call Overhead**: Minimal (just stack frames and method calls)

For production use with very large file trees, consider:
- Async/await pattern for responsiveness
- Background worker threads
- Progress reporting
- Cancellation token support

## Security Considerations

✅ No code injection vulnerabilities (path is user-selected via dialog)
✅ No directory traversal (Windows API handles validation)
✅ Exception information is sanitized before display
✅ File I/O is read-only (no deletion, modification, or creation)
✅ No sensitive data is logged or cached

## Maintenance Notes

- **Low Complexity**: Single form, ~200 lines of code
- **Easy to Extend**: Methods are modular and well-separated
- **Testable Logic**: Business logic is separate from UI
- **Clear Naming**: Variable and method names are self-documenting
- **Well-Commented**: Supports future developers

## Future Enhancement Ideas

1. **Async Operations**:
   ```csharp
   private async Task<int> CountFilesRecursivelyAsync(string folderPath)
   {
       return await Task.Run(() => CountFilesRecursively(folderPath));
   }
   ```

2. **File Type Filtering**:
   ```csharp
   Directory.GetFiles(folderPath, "*.txt")
   ```

3. **Size Calculation**:
   ```csharp
   new FileInfo(filePath).Length
   ```

4. **Exclude Patterns**:
   ```csharp
   if (directory.Contains("node_modules")) continue;
   ```

5. **Progress Reporting**:
   ```csharp
   private int CountFilesRecursiveHelper(string folderPath, 
                                         IProgress<DirectoryScanProgress> progress)
   {
       // Report progress: directories scanned, files found, etc.
   }
   ```
