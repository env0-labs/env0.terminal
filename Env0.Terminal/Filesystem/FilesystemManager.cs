using System;
using System.Collections.Generic;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Filesystem
{
    public class FilesystemManager
    {
        private FileEntry root;
        private FileEntry currentDirectory;

        public FileEntry CurrentDirectory => currentDirectory;

        public FilesystemManager(FileEntry loadedRoot)
        {
            root = loadedRoot;
            currentDirectory = root;
        }

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

        public bool GetFileContent(string path, out string content, out string error)
        {
            content = null;
            error = null;

            if (!TryGetEntry(path, out var entry, out error))
            {
                error = error ?? "No such file or directory";
                return false;
            }

            if (entry.IsDirectory)
            {
                error = "Is a directory";
                return false;
            }

            // .sh file not readable
            if (!string.IsNullOrEmpty(entry.Type) && entry.Type.ToLower().TrimStart('.') == "sh")
            {
                error = "File is executable and cannot be read";
                return false;
            }

            // File too large
            if (!string.IsNullOrEmpty(entry.Content))
            {
                var lines = entry.Content.Split('\n');
                if (lines.Length > 1000)
                {
                    error = "File too large (1,000 line limit)";
                    return false;
                }
            }

            content = string.IsNullOrEmpty(entry.Content) ? "(empty file)" : entry.Content;
            return true;
        }

        public bool ChangeDirectory(string path, out string error)
        {
            error = null;
            // TODO: hook up to debug flag
            // Console.WriteLine($"[DEBUG][CD] Attempting to cd to: '{path}' from '{currentDirectory.Name}'");

            if (string.IsNullOrWhiteSpace(path))
                path = "/";

            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            FileEntry dir = path.StartsWith("/") ? root : currentDirectory;

            // TODO: hook up to debug flag
            // Console.WriteLine($"[DEBUG][CD] Starting dir: '{dir.Name}', children: {string.Join(", ", dir.Children.Keys)}");

            foreach (var part in parts)
            {
                if (part == ".") continue;
                if (part == "..")
                {
                    if (dir.Parent != null)
                        dir = dir.Parent;
                    continue;
                }

                // TODO: hook up to debug flag
                // Console.WriteLine($"[DEBUG][CD] Looking for part '{part}' in children: {string.Join(", ", dir.Children.Keys)}");

                if (!dir.IsDirectory || !dir.Children.TryGetValue(part, out var next) || !next.IsDirectory)
                {
                    // TODO: hook up to debug flag
                    // Console.WriteLine($"[DEBUG][CD] Failed to find directory: '{part}'");
                    error = $"No such directory: {part}";
                    return false;
                }
                dir = next;
            }

            // TODO: hook up to debug flag
            // Console.WriteLine($"[DEBUG][CD] Directory change success: now at '{dir.Name}' (full path: {GetPathForEntry(dir)})");

            currentDirectory = dir;

            // TODO: hook up to debug flag
            /*
            var node = currentDirectory;
            var parents = new List<string>();
            while (node != null)
            {
                parents.Add(node.Name);
                node = node.Parent;
            }
            parents.Reverse();
            Console.WriteLine($"[DEBUG][CD] currentDirectory full parent chain: /{string.Join("/", parents)}");
            */

            return true;
        }

        public string GetCurrentDirectoryPath()
        {
            var stack = new Stack<string>();
            var dir = currentDirectory;
            while (dir != null)
            {
                if (!string.IsNullOrEmpty(dir.Name))
                    stack.Push(dir.Name);
                dir = dir.Parent;
            }
            return stack.Count == 0 ? "/" : "/" + string.Join("/", stack);
        }

        private string GetPathForEntry(FileEntry entry)
        {
            if (entry == null) return "/";
            var stack = new Stack<string>();
            var node = entry;
            while (node != null)
            {
                if (!string.IsNullOrEmpty(node.Name))
                    stack.Push(node.Name);
                node = node.Parent;
            }
            return stack.Count == 0 ? "/" : "/" + string.Join("/", stack);
        }

        public static FilesystemManager FromPocoRoot(FileEntry pocoRoot)
        {
            var fsRoot = FileEntryToFileSystemEntryConverter.Convert("", pocoRoot, null);
            return new FilesystemManager(fsRoot);
        }

        public bool TryGetEntry(string path, out FileEntry entry, out string error)
        {
            error = null;
            entry = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "No such file or directory";
                return false;
            }

            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            FileEntry node = path.StartsWith("/") ? root : currentDirectory;

            foreach (var part in parts)
            {
                if (part == ".") continue;
                if (part == "..")
                {
                    node = node?.Parent ?? node;
                    continue;
                }

                if (node?.Children == null || !node.Children.TryGetValue(part, out node))
                {
                    error = "No such file or directory";
                    return false;
                }
            }

            entry = node;
            return true;
        }

        public bool TryGetDirectory(string path, out FileEntry dir, out string error)
        {
            dir = null;
            error = null;
            if (!TryGetEntry(path, out var entry, out error))
            {
                error = error ?? "No such directory";
                return false;
            }

            if (!entry.IsDirectory)
            {
                error = "Not a directory";
                return false;
            }

            dir = entry;
            return true;
        }
    }
}
