using Env0.Terminal;
using System;

class Program
{
    static void Main(string[] args)
    {
        // Use the same BuildTestFilesystem from your tests, or recreate a similar tree here:
        var root = new FileSystemEntry
        {
            Name = "root",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir"
        };
        var home = new FileSystemEntry
        {
            Name = "home",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = root
        };
        var user = new FileSystemEntry
        {
            Name = "user",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase),
            Type = "dir",
            Parent = home
        };

        // Wire up the tree
        user.Children.Add("welcome.txt", welcomeTxt);
        home.Children.Add("user", user);
        root.Children.Add("home", home);

        var fs = new FilesystemManager(root);

        Console.WriteLine($"Current dir: {fs.CurrentDirectoryName()}");

        string error;
        fs.ChangeDirectory("home/user", out error);
        Console.WriteLine($"After cd home/user: {fs.CurrentDirectoryName()}");

        fs.ChangeDirectory("..", out error);
        Console.WriteLine($"After cd ..: {fs.CurrentDirectoryName()}");

        string content;
        fs.GetFileContent("welcome.txt", out content, out error);
        Console.WriteLine($"cat welcome.txt: {content}");

        // Interactive playground (uncomment to play live)
        
       /* while (true)
        {
            Console.Write($"{fs.CurrentDirectoryName()}> ");
            var cmd = Console.ReadLine();
            if (cmd == "exit") break;
            if (cmd.StartsWith("cd "))
            {
                if (fs.ChangeDirectory(cmd.Substring(3), out error))
                    Console.WriteLine("OK!");
                else
                    Console.WriteLine($"ERROR: {error}");
            }
            else if (cmd.StartsWith("cat "))
            {
                if (fs.GetFileContent(cmd.Substring(4), out content, out error))
                    Console.WriteLine(content);
                else
                    Console.WriteLine($"ERROR: {error}");
            }
            else if (cmd == "ls")
            {
                var items = fs.ListCurrentDirectory();
                Console.WriteLine(string.Join("  ", items));
            }
            else
            {
                Console.WriteLine("Unknown command. Try ls, cd <dir>, cat <file>, or exit.");
            }
        }
        */
    }
}
