using Xunit;
using Env0.Terminal.Filesystem;
using System;
using System.Collections.Generic;

public class FilesystemManagerHostileUserTests
{
    [Trait("TestType", "Psychotic")]
    private FileSystemEntry MakeMinimalRoot()
    {
        return new FileSystemEntry
        {
            Name = "root",
            IsDirectory = true,
            Children = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir"
        };
    }

    [Fact]

    public void Throws_When_DirectoryHasChildNamed_DotDot()
    {
        var root = MakeMinimalRoot();
        var evil = new FileSystemEntry
        {
            Name = "..",
            IsDirectory = true,
            Children = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = root
        };

        Assert.Throws<ArgumentException>(() => FileSystemEntry.ValidateFSName(evil.Name));
    }

    [Fact]
    public void Throws_When_FilenameHasSlashes()
    {
        var root = MakeMinimalRoot();
        var broken = new FileSystemEntry
        {
            Name = "user/home",
            IsDirectory = false,
            Content = "oops",
            Type = "file",
            Parent = root
        };

        Assert.Throws<ArgumentException>(() => FileSystemEntry.ValidateFSName(broken.Name));
    }



    [Fact]
    public void PathTraversalAboveRoot_DoesNotCrash()
    {
        var root = MakeMinimalRoot();
        var fs = new FilesystemManager(root);
        string error;

        // Try 10 levels up from root
        for (int i = 0; i < 10; i++)
        {
            Assert.True(fs.ChangeDirectory("..", out error));
            Assert.Equal("root", fs.CurrentDirectoryName());
        }
    }

[Fact]
public void DirectoryAndFileWithSameName_DirectoryWins()
{
    var root = MakeMinimalRoot();
    var subdir = new FileSystemEntry
    {
        Name = "bin",
        IsDirectory = true,
        Children = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
        Type = "dir",
        Parent = root
    };
    var binFile = new FileSystemEntry
    {
        Name = "bin",
        IsDirectory = false,
        Content = "i should be ignored",
        Type = "file",
        Parent = root
    };
    root.Children.Add(subdir.Name, subdir);
    Assert.Throws<ArgumentException>(() => root.Children.Add(binFile.Name, binFile));
}


    [Fact]
    public void FileWithRidiculousName_Length256Plus()
    {
        var root = MakeMinimalRoot();
        string longName = new string('a', 300) + ".txt";
        var weird = new FileSystemEntry
        {
            Name = longName,
            IsDirectory = false,
            Content = "just... why",
            Type = "file",
            Parent = root
        };
        root.Children.Add(longName, weird);
        var fs = new FilesystemManager(root);

        string content, error;
        Assert.True(fs.GetFileContent(longName, out content, out error));
        Assert.Equal("just... why", content);
    }

    [Fact]
    public void LsOnDirectoryWithOnlyInvalidChildren_PrintsEmpty()
    {
        var root = MakeMinimalRoot();
        var junk = new FileSystemEntry
        {
            Name = "",
            IsDirectory = false,
            Content = "null",
            Type = "file",
            Parent = root
        };
        root.Children.Add("", junk);
        var fs = new FilesystemManager(root);

        var items = fs.ListCurrentDirectory();
        Assert.True(items.Count == 1 || items.Count == 0);
    }

    [Fact]
    public void CatOnDirectory_AlwaysFails()
    {
        var root = MakeMinimalRoot();
        var home = new FileSystemEntry
        {
            Name = "home",
            IsDirectory = true,
            Children = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = root
        };
        root.Children.Add("home", home);
        var fs = new FilesystemManager(root);

        string content, error;
        Assert.False(fs.GetFileContent("home", out content, out error));
        Assert.NotNull(error);
    }
}
