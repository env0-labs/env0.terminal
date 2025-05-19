using System;
using System.Collections.Generic;

namespace Env0.Terminal
{
public class FileSystemEntry
{
    public string Name { get; set; } = "";
    public bool IsDirectory { get; set; }
    public Dictionary<string, FileSystemEntry> Children { get; set; } = new Dictionary<string, FileSystemEntry>();
    public string Content { get; set; } = "";
    public string Type { get; set; } = "";
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

        // Normalize input and split by /
        var target = path.Trim().ToLowerInvariant();
        string[] parts;
        if (target.StartsWith("/"))
        {
            // Absolute: start from root
            parts = target.Substring(1).Split('/', StringSplitOptions.RemoveEmptyEntries);
            currentDirectory = root;
        }
        else
        {
            // Relative: start from current
            parts = target.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        var dir = currentDirectory;
        foreach (var part in parts)
        {
            if (part == ".") continue;
            if (part == "..")
            {
                // Not implemented: up one level (optional—requires parent tracking)
                error = "`cd ..` not yet implemented.";
                return false;
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

public bool GetFileContent(string filename, out string content, out string error)
{
    content = null;
    error = null;
    if (string.IsNullOrWhiteSpace(filename))
    {
        error = "No file specified.";
        return false;
    }

    var name = filename.Trim().ToLowerInvariant();

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



        // TODO: etc.
    }
}
