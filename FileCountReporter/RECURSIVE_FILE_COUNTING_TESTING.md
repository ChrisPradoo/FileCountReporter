# Recursive File Counting - Debugging & Testing Guide

## 🧪 Testing the Recursive Method

This guide shows how to test, debug, and verify the recursive file counting method.

---

## 📋 Test Scenarios

### Test 1: Empty Directory

**Setup**:
```csharp
// Create empty folder
Directory.CreateDirectory(@"C:\Test\Empty");
```

**Test Code**:
```csharp
string path = @"C:\Test\Empty";
int result = CountFilesRecursively(path);

// Expected: 0
// Actual: ?
Assert.AreEqual(0, result);
```

**Expected Behavior**:
- GetFiles() returns empty array
- GetDirectories() returns empty array
- Function returns 0
- No recursion occurs

---

### Test 2: Single File

**Setup**:
```csharp
Directory.CreateDirectory(@"C:\Test\SingleFile");
File.Create(@"C:\Test\SingleFile\file.txt").Dispose();
```

**Test Code**:
```csharp
string path = @"C:\Test\SingleFile";
int result = CountFilesRecursively(path);

// Expected: 1
Assert.AreEqual(1, result);
```

**Expected Behavior**:
- GetFiles() returns array with 1 element
- GetDirectories() returns empty array
- Function returns 1
- No recursion occurs

---

### Test 3: Multiple Files in Root

**Setup**:
```csharp
Directory.CreateDirectory(@"C:\Test\MultiFile");
for (int i = 0; i < 5; i++)
{
    File.Create($@"C:\Test\MultiFile\file{i}.txt").Dispose();
}
```

**Test Code**:
```csharp
string path = @"C:\Test\MultiFile";
int result = CountFilesRecursively(path);

// Expected: 5
Assert.AreEqual(5, result);
```

**Expected Behavior**:
- Counts all files at root level
- No subdirectories
- Returns correct count

---

### Test 4: Nested Directories

**Setup**:
```csharp
// C:\Test\Nested
// ├─ file1.txt
// ├─ SubA
// │  └─ file2.txt
// └─ SubB
//    └─ file3.txt

Directory.CreateDirectory(@"C:\Test\Nested\SubA");
Directory.CreateDirectory(@"C:\Test\Nested\SubB");
File.Create(@"C:\Test\Nested\file1.txt").Dispose();
File.Create(@"C:\Test\Nested\SubA\file2.txt").Dispose();
File.Create(@"C:\Test\Nested\SubB\file3.txt").Dispose();
```

**Test Code**:
```csharp
string path = @"C:\Test\Nested";
int result = CountFilesRecursively(path);

// Expected: 3 (file1, file2 in SubA, file3 in SubB)
Assert.AreEqual(3, result);
```

**Expected Behavior**:
- Counts 1 file at root
- Recursively counts SubA (1 file)
- Recursively counts SubB (1 file)
- Total: 3

---

### Test 5: Deep Nesting

**Setup**:
```csharp
// Create structure like:
// Level1
// ├─ file.txt
// └─ Level2
//    ├─ file.txt
//    └─ Level3
//       ├─ file.txt
//       └─ Level4
//          └─ file.txt

string basePath = @"C:\Test\Deep";
string currentPath = basePath;

for (int level = 1; level <= 4; level++)
{
    Directory.CreateDirectory(currentPath);
    File.Create(Path.Combine(currentPath, "file.txt")).Dispose();
    currentPath = Path.Combine(currentPath, $"Level{level + 1}");
}
```

**Test Code**:
```csharp
string path = @"C:\Test\Deep";
int result = CountFilesRecursively(path);

// Expected: 4 (one at each level)
Assert.AreEqual(4, result);
```

**Expected Behavior**:
- Recursion depth = 4
- Each level has 1 file
- Accumulates: 1 + 1 + 1 + 1 = 4
- Completes quickly

---

### Test 6: Large Directory

**Setup**:
```csharp
// Create 1000 files in one directory
Directory.CreateDirectory(@"C:\Test\Large");
for (int i = 0; i < 1000; i++)
{
    File.Create($@"C:\Test\Large\file{i:D4}.txt").Dispose();
}
```

**Test Code**:
```csharp
var stopwatch = Stopwatch.StartNew();
string path = @"C:\Test\Large";
int result = CountFilesRecursively(path);
stopwatch.Stop();

// Expected: 1000
Assert.AreEqual(1000, result);

// Should complete in < 100ms
Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100);
```

**Expected Behavior**:
- Counts all 1000 files
- Completes quickly (I/O bound)
- No recursion needed

---

### Test 7: Permission Denied (Subdirectory)

**Setup**:
```csharp
// Create structure where SubRestricted is not readable
// C:\Test\PermissionTest
// ├─ file1.txt (readable)
// ├─ SubRestricted (NOT readable)
// │  └─ file2.txt (inaccessible)
// └─ SubNormal (readable)
//    └─ file3.txt (readable)

Directory.CreateDirectory(@"C:\Test\PermissionTest\SubRestricted");
Directory.CreateDirectory(@"C:\Test\PermissionTest\SubNormal");
File.Create(@"C:\Test\PermissionTest\file1.txt").Dispose();
File.Create(@"C:\Test\PermissionTest\SubNormal\file3.txt").Dispose();

// Deny permissions on SubRestricted
var dirInfo = new DirectoryInfo(@"C:\Test\PermissionTest\SubRestricted");
var security = dirInfo.GetAccessControl();
security.AddAccessRule(new FileSystemAccessRule(
    @"EVERYONE",
    FileSystemRights.ListDirectory,
    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
    PropagationFlags.None,
    AccessControlType.Deny));
dirInfo.SetAccessControl(security);
```

**Test Code**:
```csharp
string path = @"C:\Test\PermissionTest";
int result = CountFilesRecursively(path);

// Expected: 2 (file1 + file3, but NOT file2 in inaccessible SubRestricted)
// This tests the catch-and-continue pattern
Assert.AreEqual(2, result);

// No exception should be thrown
// Partial results should be returned
```

**Expected Behavior**:
- Counts file1.txt at root (1)
- Tries to access SubRestricted
- UnauthorizedAccessException caught
- Continues to SubNormal
- Counts file3.txt (1)
- Total: 2 (not all files, but no crash)

---

### Test 8: Path Does Not Exist

**Test Code**:
```csharp
string path = @"C:\NonExistentPath\DoesNotExist";
try
{
    int result = CountFilesRecursively(path);
    Assert.Fail("Should have thrown DirectoryNotFoundException");
}
catch (DirectoryNotFoundException)
{
    // Expected - this is good
}
```

**Expected Behavior**:
- Throws DirectoryNotFoundException
- Caller should validate path exists first

---

### Test 9: Invalid Path Characters

**Test Code**:
```csharp
string path = @"C:\Invalid<Path>";
try
{
    int result = CountFilesRecursively(path);
    Assert.Fail("Should have thrown ArgumentException");
}
catch (ArgumentException)
{
    // Expected - invalid characters
}
```

**Expected Behavior**:
- Throws ArgumentException
- .NET validates path syntax

---

### Test 10: Circular Symlinks

**Setup**:
```csharp
// Create circular symlink (careful with this!)
// C:\Test\Circular
// ├─ file.txt
// └─ Link → points back to parent

// This is advanced and can cause infinite loops
// Only test if you handle circular links
```

**Note**: The current implementation does NOT handle circular symlinks.
If you need to handle them, you must track visited directories:

```csharp
private int CountFilesWithCircularDetection(string folderPath)
{
    var visited = new HashSet<string>();
    return CountFilesWithCircularDetectionHelper(folderPath, visited);
}

private int CountFilesWithCircularDetectionHelper(string folderPath,
                                                  HashSet<string> visited)
{
    // Get full path to detect symlink loops
    string fullPath = Path.GetFullPath(folderPath);
    
    // If already visited, skip (prevents infinite loop)
    if (visited.Contains(fullPath))
    {
        return 0;
    }
    
    visited.Add(fullPath);
    
    int fileCount = 0;

    try
    {
        fileCount += Directory.GetFiles(folderPath).Length;
        string[] subdirectories = Directory.GetDirectories(folderPath);

        foreach (string subdirectory in subdirectories)
        {
            try
            {
                fileCount += CountFilesWithCircularDetectionHelper(subdirectory, visited);
            }
            catch (UnauthorizedAccessException)
            {
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
```

---

## 🐛 Debugging Techniques

### Technique 1: Console Output Tracing

```csharp
private int CountFilesRecursiveDebug(string folderPath)
{
    Console.WriteLine($"→ Entering: {folderPath}");
    
    int fileCount = 0;

    try
    {
        string[] files = Directory.GetFiles(folderPath);
        fileCount += files.Length;
        Console.WriteLine($"  Files found: {files.Length}");

        string[] subdirectories = Directory.GetDirectories(folderPath);
        Console.WriteLine($"  Subdirectories: {subdirectories.Length}");

        foreach (string subdirectory in subdirectories)
        {
            try
            {
                Console.WriteLine($"  Processing: {Path.GetFileName(subdirectory)}");
                fileCount += CountFilesRecursiveDebug(subdirectory);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"  ⚠️ Skipped: {Path.GetFileName(subdirectory)} (access denied)");
                continue;
            }
        }
    }
    catch (UnauthorizedAccessException)
    {
        throw;
    }

    Console.WriteLine($"← Exiting: {folderPath} (count: {fileCount})");
    return fileCount;
}
```

**Output Example**:
```
→ Entering: C:\Test\Debug
  Files found: 1
  Subdirectories: 2
  Processing: SubA
  → Entering: C:\Test\Debug\SubA
    Files found: 1
    Subdirectories: 0
  ← Exiting: C:\Test\Debug\SubA (count: 1)
  Processing: SubB
  → Entering: C:\Test\Debug\SubB
    Files found: 1
    Subdirectories: 0
  ← Exiting: C:\Test\Debug\SubB (count: 1)
← Exiting: C:\Test\Debug (count: 3)
```

---

### Technique 2: Visual Studio Debugger

**Using breakpoints**:

1. Open Form1.cs
2. Click in the margin next to `fileCount += Directory.GetFiles(folderPath).Length;`
3. A red dot appears (breakpoint)
4. Run the application
5. Click Scan
6. Debugger pauses at breakpoint
7. Use **Debug → Step Into** (F11) to trace execution
8. Watch `fileCount` variable in Locals window

**Debugging windows**:
- **Locals**: See current variables
- **Watch**: Monitor specific variables
- **Call Stack**: See function call chain
- **Output**: See debug output

---

### Technique 3: Instrumentation with Counters

```csharp
public class FileCounterWithStats
{
    public int TotalFiles { get; set; }
    public int DirectoriesScanned { get; set; }
    public int AccessDeniedCount { get; set; }
    public TimeSpan Duration { get; set; }
}

private FileCounterWithStats CountFilesWithStats(string folderPath)
{
    var stats = new FileCounterWithStats();
    var stopwatch = Stopwatch.StartNew();

    stats.TotalFiles = CountFilesWithStatsHelper(folderPath, stats);

    stopwatch.Stop();
    stats.Duration = stopwatch.Elapsed;

    return stats;
}

private int CountFilesWithStatsHelper(string folderPath, 
                                       FileCounterWithStats stats)
{
    int fileCount = 0;

    try
    {
        stats.DirectoriesScanned++;
        fileCount += Directory.GetFiles(folderPath).Length;

        foreach (string subdirectory in Directory.GetDirectories(folderPath))
        {
            try
            {
                fileCount += CountFilesWithStatsHelper(subdirectory, stats);
            }
            catch (UnauthorizedAccessException)
            {
                stats.AccessDeniedCount++;
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
```

**Usage**:
```csharp
var stats = CountFilesWithStats(@"C:\MyFolder");
Console.WriteLine($"Files: {stats.TotalFiles}");
Console.WriteLine($"Directories: {stats.DirectoriesScanned}");
Console.WriteLine($"Access Denied: {stats.AccessDeniedCount}");
Console.WriteLine($"Time: {stats.Duration.TotalSeconds:F2}s");
```

---

## 📊 Performance Profiling

### Measure Execution Time

```csharp
private void ProfileCountingMethod()
{
    string testPath = @"C:\LargeFolder";
    var stopwatch = Stopwatch.StartNew();
    
    int result = CountFilesRecursively(testPath);
    
    stopwatch.Stop();
    
    Console.WriteLine($"Found: {result} files");
    Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"Rate: {result / stopwatch.Elapsed.TotalSeconds:F0} files/sec");
}
```

**Sample Output**:
```
Found: 50,000 files
Time: 3,240ms
Rate: 15,432 files/sec
```

### Compare Implementations

```csharp
private void CompareImplementations()
{
    string testPath = @"C:\TestFolder";

    // Test Version 1: Simple
    var sw1 = Stopwatch.StartNew();
    int result1 = CountFilesSimple(testPath);
    sw1.Stop();

    // Test Version 2: Production
    var sw2 = Stopwatch.StartNew();
    int result2 = CountFilesRecursively(testPath);
    sw2.Stop();

    Console.WriteLine($"Simple:      {sw1.ElapsedMilliseconds}ms (result: {result1})");
    Console.WriteLine($"Production:  {sw2.ElapsedMilliseconds}ms (result: {result2})");
}
```

---

## ✅ Test Checklist

Before deploying, verify:

- [ ] Empty directory returns 0
- [ ] Single file returns 1
- [ ] Multiple files counted correctly
- [ ] Subdirectories traversed
- [ ] Deep nesting works
- [ ] Large directories handled quickly
- [ ] Permission errors handled gracefully
- [ ] Partial results returned on access denied
- [ ] No infinite loops
- [ ] Stack doesn't overflow
- [ ] Performance acceptable

---

## 🎯 Common Issues & Solutions

### Issue 1: Stack Overflow
**Symptom**: `StackOverflowException` after long wait  
**Cause**: Too many nested directories  
**Solution**:
1. Check directory depth (use depth limit)
2. Implement circular symlink detection
3. Use iterative instead of recursive approach

### Issue 2: Slow Performance
**Symptom**: Takes >10 seconds for 100K files  
**Cause**: I/O bottleneck or network drive  
**Solution**:
1. Run on local SSD instead of network drive
2. Use async implementation (version 8)
3. Profile to find actual bottleneck

### Issue 3: Returns Wrong Count
**Symptom**: Expected 1000 but got 999  
**Cause**: Access denied to one folder (not reported)  
**Solution**: This is actually correct behavior! Access denied folders are skipped

### Issue 4: Hangs/Freezes UI
**Symptom**: Application becomes unresponsive  
**Cause**: Long-running operation on UI thread  
**Solution**: Use async implementation (version 8)

### Issue 5: Out of Memory
**Symptom**: Memory usage grows unbounded  
**Cause**: Returning list of all files instead of count  
**Solution**: Use version 2 (count) instead of version 7 (list)

---

## 🚀 Best Practices

✅ **DO**:
- Test with various directory structures
- Profile performance on realistic data
- Handle errors gracefully
- Provide user feedback during scanning
- Validate input path exists first

❌ **DON'T**:
- Test only with empty directories
- Assume 100% file permissions
- Block the UI thread
- Recursively scan symlinks without detection
- Ignore error conditions

---

**Version**: 1.0  
**Purpose**: Testing, debugging, and profiling guide
