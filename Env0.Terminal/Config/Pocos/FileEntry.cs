using Newtonsoft.Json;
using System.Collections.Generic;

namespace Env0.Terminal.Config.Pocos
{
    [JsonConverter(typeof(FileEntryConverter))]
    public class FileEntry
    {
        // If "type" == "file", it's a file node; otherwise, it's a directory node.
        public string Type { get; set; } = ""; // "file" or "" (directory)
        public string Content { get; set; }   // Only for files; always null for dirs
        public Dictionary<string, FileEntry> Children { get; set; } // Only for dirs; always null for files
    }
}
