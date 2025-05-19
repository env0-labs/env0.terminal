using System;
using System.Collections.Generic;

namespace Env0.Terminal
{
    public class FileSystemEntry
    {
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public Dictionary<string, FileSystemEntry> Children { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
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

        // TODO: Add ChangeDirectory, GetFileContent, etc.
    }
}
