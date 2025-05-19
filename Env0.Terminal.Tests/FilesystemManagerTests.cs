using Xunit;
using System.Collections.Generic;
using Env0.Terminal;

public class FilesystemManagerTests
{
private FileSystemEntry BuildTestFilesystem()
{
    // Build nodes
    var welcomeTxt = new FileSystemEntry
    {
        Name = "welcome.txt",
        IsDirectory = false,
        Content = "Welcome to your basic UNIX machine!",
        Type = "file"
    };
    var userDir = new FileSystemEntry
    {
        Name = "user",
        IsDirectory = true,
        Children = new Dictionary<string, FileSystemEntry>
        {
            { "welcome.txt", welcomeTxt }
        },
        Type = "dir"
    };
    var homeDir = new FileSystemEntry
    {
        Name = "home",
        IsDirectory = true,
        Children = new Dictionary<string, FileSystemEntry>
        {
            { "user", userDir }
        },
        Type = "dir"
    };
    var hostnameTxt = new FileSystemEntry
    {
        Name = "hostname.txt",
        IsDirectory = false,
        Content = "Basic UNIX Machine",
        Type = "file"
    };
    var etcDir = new FileSystemEntry
    {
        Name = "etc",
        IsDirectory = true,
        Children = new Dictionary<string, FileSystemEntry>
        {
            { "hostname.txt", hostnameTxt }
        },
        Type = "dir"
    };
    var root = new FileSystemEntry
    {
        Name = "root",
        IsDirectory = true,
        Children = new Dictionary<string, FileSystemEntry>
        {
            { "home", homeDir },
            { "etc", etcDir }
        },
        Type = "dir"
    };

    // Set parent pointers
    welcomeTxt.Parent = userDir;
    userDir.Parent = homeDir;
    homeDir.Parent = root;
    hostnameTxt.Parent = etcDir;
    etcDir.Parent = root;
    root.Parent = null; // Explicitly state root has no parent

    return root;
}


    [Fact]
    public void ListCurrentDirectory_AtRoot_ListsHomeAndEtc()
    {
        var root = BuildTestFilesystem();
        var fs = new FilesystemManager(root);

        var items = fs.ListCurrentDirectory();

        Assert.Contains("home", items);
        Assert.Contains("etc", items);
        Assert.Equal(2, items.Count);
    }

[Fact]
public void ChangeDirectory_ToSubdirectory_Works()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string error;
    var ok = fs.ChangeDirectory("home", out error);

    Assert.True(ok);
    Assert.Null(error);
    Assert.Equal("home", fs.CurrentDirectoryName());
}

[Fact]
public void ChangeDirectory_ToInvalidDirectory_Fails()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string error;
    var ok = fs.ChangeDirectory("doesnotexist", out error);

    Assert.False(ok);
    Assert.NotNull(error);
    Assert.Equal("No such directory: doesnotexist", error);
}

[Fact]
public void GetFileContent_ExistingFile_ReturnsContent()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string content, error;
    fs.ChangeDirectory("home/user", out _);
    var ok = fs.GetFileContent("welcome.txt", out content, out error);

    Assert.True(ok);
    Assert.Null(error);
    Assert.Equal("Welcome to your basic UNIX machine!", content);
}

[Fact]
public void GetFileContent_Directory_ReturnsError()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    fs.ChangeDirectory("home", out _);
    string content, error;
    var ok = fs.GetFileContent("user", out content, out error);

    Assert.False(ok);
    Assert.Equal("user is a directory.", error);
}

[Fact]
public void GetFileContent_MissingFile_ReturnsError()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string content, error;
    var ok = fs.GetFileContent("nofile.txt", out content, out error);

    Assert.False(ok);
    Assert.Equal("No such file: nofile.txt", error);
}

[Fact]
public void ChangeDirectory_ParentDirectory_Works()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string error;
    fs.ChangeDirectory("home/user", out error);
    Assert.Equal("user", fs.CurrentDirectoryName());

    fs.ChangeDirectory("..", out error);
    Assert.Equal("home", fs.CurrentDirectoryName());

    fs.ChangeDirectory("..", out error);
    Assert.Equal("root", fs.CurrentDirectoryName());

    // Attempt to go above root
    fs.ChangeDirectory("..", out error);
    Assert.Equal("root", fs.CurrentDirectoryName());
}




[Fact]
public void ChangeDirectory_AbsolutePath_WorksFromRoot()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string error;
    var ok = fs.ChangeDirectory("/home/user", out error);

    Assert.True(ok);
    Assert.Null(error);
    Assert.Equal("user", fs.CurrentDirectoryName());
}

[Fact]
public void ChangeDirectory_AbsolutePath_WorksFromAnyDirectory()
{
    var root = BuildTestFilesystem();
    var fs = new FilesystemManager(root);

    string error;
    fs.ChangeDirectory("home/user", out error);
    Assert.Equal("user", fs.CurrentDirectoryName());

    // Now cd to absolute path from within a subdirectory
    var ok = fs.ChangeDirectory("/etc", out error);

    Assert.True(ok);
    Assert.Null(error);
    Assert.Equal("etc", fs.CurrentDirectoryName());
}
}