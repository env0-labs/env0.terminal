using System;
using System.Collections.Generic;

namespace Env0.Terminal.Filesystem
{
    public class FileSystemEntry
    {
        public string Name { get; set; } = "";
        public bool IsDirectory { get; set; }
        public Dictionary<string, FileSystemEntry> Children { get; set; } = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase);
        public string Content { get; set; } = "";
        public string Type { get; set; } = "";
        public FileSystemEntry Parent { get; set; } // Null for root
        public static void ValidateFSName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty or whitespace.");
            if (name == "." || name == "..")
                throw new ArgumentException("Dot and dot-dot are reserved names.");
            if (name.Contains("/") || name.Contains("\\"))
                throw new ArgumentException("Names cannot contain slashes.");
            // Add any other rules here, e.g., length > 256, Unicode, etc.
        }
    }

    public class FilesystemManager
    {
        private FileSystemEntry root;
        private FileSystemEntry currentDirectory;

        public FilesystemManager(FileSystemEntry loadedRoot)
        {
            root = loadedRoot;
            currentDirectory = root;
        }

        // Example method: List current directory
        public List<string> ListCurrentDirectory()
        {
            if (!currentDirectory.IsDirectory)
                throw new InvalidOperationException("Current entry is not a directory.");

            return new List<string>(currentDirectory.Children.Keys);
        }

        public string CurrentDirectoryName()
        {
            return currentDirectory.Name;
        }

        public bool ChangeDirectory(string path, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "No path specified.";
                return false;
            }

            var target = path.Trim();
            string[] parts;
            FileSystemEntry dir;

            if (target.StartsWith("/"))
            {
                dir = root;
                parts = target.Substring(1).Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                dir = currentDirectory;
                parts = target.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (var part in parts)
            {
                if (part == ".") continue;
                if (part == "..")
                {
                    if (dir.Parent != null)
                        dir = dir.Parent;
                    continue;
                }
                if (!dir.IsDirectory || !dir.Children.TryGetValue(part, out var next) || !next.IsDirectory)
                {
                    error = $"No such directory: {part}";
                    return false;
                }
                dir = next;
            }

            currentDirectory = dir;
            return true;
        }

        // Helper to build full path by walking up parents to the root
            
        public string GetCurrentDirectoryPath()
        {
            var stack = new Stack<string>();
            var dir = currentDirectory;
            while (dir != null && dir.Parent != null)
            {
                stack.Push(dir.Name);
                dir = dir.Parent;
            }
            // If at root, stack will be empty. Represent as "/"
            return stack.Count == 0 ? "/" : "/" + string.Join("/", stack);
        }
        
        public bool GetFileContent(string filename, out string content, out string error)
        {
            content = null;
            error = null;
            if (string.IsNullOrWhiteSpace(filename))
            {
                error = "No file specified.";
                return false;
            }

            var name = filename.Trim();

            if (!currentDirectory.IsDirectory || !currentDirectory.Children.TryGetValue(name, out var entry))
            {
                error = $"No such file: {filename}";
                return false;
            }

            if (entry.IsDirectory)
            {
                error = $"{filename} is a directory.";
                return false;
            }

            // Check for .sh
            if (entry.Name.EndsWith(".sh", StringComparison.OrdinalIgnoreCase))
            {
                error = "file is executable and cannot be read";
                return false;
            }

            
            // File too large (per spec: max 1000 lines)
            var lines = entry.Content?.Split('\n');
            if (lines != null && lines.Length > 1000)
            {
                error = "bash: read: File too large (1,000 line limit).";
                return false;
            }

            // Empty file
            if (string.IsNullOrWhiteSpace(entry.Content))
            {
                content = "(empty file)";
                return true;
            }

            content = entry.Content;
            return true;
        }

        // ---------- NEW HELPER BELOW ----------
        // This does NOT mutate any state. Use for path-based 'ls', 'cat', etc.

        public bool TryGetDirectory(string path, out FileSystemEntry dir, out string error)
        {
            dir = null;
            error = null;

            if (string.IsNullOrWhiteSpace(path) || path == ".")
            {
                dir = currentDirectory;
                return true;
            }

            FileSystemEntry start;
            string[] parts;
            if (path.StartsWith("/"))
            {
                start = root;
                parts = path.Substring(1).Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                start = currentDirectory;
                parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var temp = start;
            foreach (var part in parts)
            {
                if (part == ".") continue;
                if (part == "..")
                {
                    if (temp.Parent != null)
                        temp = temp.Parent;
                    continue;
                }
                if (!temp.IsDirectory || !temp.Children.TryGetValue(part, out var next) || !next.IsDirectory)
                {
                    error = $"No such directory: {part}";
                    return false;
                }
                temp = next;
            }

            dir = temp;
            return true;
        }

        // --------------------------------------
        // TODO: etc.
    }
}
