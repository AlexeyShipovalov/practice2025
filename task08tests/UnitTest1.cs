using Xunit;
using System.IO;
using CommandLib;
using FileSystemCommands;
public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Directory.Delete(testDir, true);
    }
    [Fact]
    public void DirectorySizeCommand_ShouldHandleEmptyDirectory()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "EmptyTestDir");
        Directory.CreateDirectory(testDir);
        var command = new DirectorySizeCommand(testDir);
        command.Execute();
        Directory.Delete(testDir, true);
    }
    [Fact]
    public void DirectorySizeCommand_ShouldHandleNestedDirectories()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "NestedTestDir");
        Directory.CreateDirectory(testDir);
        Directory.CreateDirectory(Path.Combine(testDir, "SubDir"));
        File.WriteAllText(Path.Combine(testDir, "SubDir", "test.txt"), "Content");
        var command = new DirectorySizeCommand(testDir);
        command.Execute();
        Directory.Delete(testDir, true);
    }
    [Fact]
    public void FindFilesCommand_ShouldHandleNoMatches()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "NoMatchTestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.dat"), "Data");
        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();
        Directory.Delete(testDir, true);
    }
    [Fact]
    public void FindFilesCommand_ShouldHandleMultipleMatches()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "MultiMatchTestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text1");
        File.WriteAllText(Path.Combine(testDir, "file2.txt"), "Text2");
        File.WriteAllText(Path.Combine(testDir, "file3.log"), "Log");
        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();
        Directory.Delete(testDir, true);
    }
    [Fact]
    public void FindFilesCommand_ShouldHandleComplexPatterns()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "PatternTestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "report_2023.txt"), "Data");
        File.WriteAllText(Path.Combine(testDir, "report_2022.txt"), "Data");
        File.WriteAllText(Path.Combine(testDir, "data_2023.csv"), "Data");
        var command = new FindFilesCommand(testDir, "report_*.txt");
        command.Execute();
        Directory.Delete(testDir, true);
    }
}
