using Env0.Terminal;
using System;

class Program
{
    static void Main(string[] args)
    {
        // Build root directory
        var root = new FileSystemEntry
        {
            Name = "root",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir"
        };

        // Build home directory
        var home = new FileSystemEntry
        {
            Name = "home",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = root
        };

        // Build user directory
        var user = new FileSystemEntry
        {
            Name = "user",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = home
        };

        // Build welcome.txt file
        var welcomeTxt = new FileSystemEntry
        {
            Name = "welcome.txt",
            IsDirectory = false,
            Content = "Welcome to your basic UNIX machine!",
            Type = "file",
            Parent = user
        };

        // Build etc directory
        var etc = new FileSystemEntry
        {
            Name = "etc",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = root
        };

        // Build hostname.txt file
        var hostnameTxt = new FileSystemEntry
        {
            Name = "hostname.txt",
            IsDirectory = false,
            Content = "Basic UNIX Machine",
            Type = "file",
            Parent = etc
        };

        // Wire up the tree
        user.Children.Add("welcome.txt", welcomeTxt);
        home.Children.Add("user", user);
        root.Children.Add("home", home);

        etc.Children.Add("hostname.txt", hostnameTxt);
        root.Children.Add("etc", etc);

        // Create the FilesystemManager with root
        var fs = new FilesystemManager(root);

        // Playground: simple interactive loop
        while (true)
        {
            Console.Write($"{fs.CurrentDirectoryName()}> ");
            var cmd = Console.ReadLine();
            if (cmd == "exit") break;

            if (cmd.StartsWith("cd "))
            {
                string error;
                if (fs.ChangeDirectory(cmd.Substring(3), out error))
                    Console.WriteLine("OK!");
                else
                    Console.WriteLine($"ERROR: {error}");
            }
            else if (cmd.StartsWith("cat "))
            {
                string content, error;
                if (fs.GetFileContent(cmd.Substring(4), out content, out error))
                    Console.WriteLine(content);
                else
                    Console.WriteLine($"ERROR: {error}");
            }
            else if (cmd == "ls")
            {
                var items = fs.ListCurrentDirectory();
                if (items.Count == 0)
                    Console.WriteLine("(empty)");
                else
                    Console.WriteLine(string.Join("  ", items));
            }
            else
            {
                Console.WriteLine("Unknown command. Try ls, cd <dir>, cat <file>, or exit.");
            }
        }
    }
}
