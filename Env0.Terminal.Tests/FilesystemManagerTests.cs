using Xunit;
using System.Collections.Generic;
using Env0.Terminal;

public class FilesystemManagerTests
{
    private FileSystemEntry BuildTestFilesystem()
    {
        // / (root)
        // ├── home/
        // │   └── user/
        // │       └── welcome.txt
        // └── etc/
        //     └── hostname.txt

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
}
