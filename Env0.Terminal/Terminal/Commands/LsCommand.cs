using Env0.Terminal.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Env0.Terminal.Terminal.Commands
{
    public class LsCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            var fsManager = session.FilesystemManager;
            string targetDir = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? session.CurrentWorkingDirectory // USE CWD instead of "."
                : args[0];

            // If someone literally types ".", treat as CWD
            if (targetDir == ".")
                targetDir = session.CurrentWorkingDirectory;

            // TODO: hook up to debug flag
            // Console.WriteLine($"[DEBUG][LS] Listing directory: '{targetDir}'");

            if (!fsManager.TryGetDirectory(targetDir, out var dir, out var error))
            {
                // TODO: hook up to debug flag
                // Console.WriteLine($"[DEBUG][LS] TryGetDirectory failed: {error}");
                return new CommandResult($"bash: ls: {error}\n\n", isError: true);
            }

            if (!dir.IsDirectory)
            {
                // TODO: hook up to debug flag
                // Console.WriteLine($"[DEBUG][LS] Not a directory: {targetDir}");
                return new CommandResult($"bash: ls: Not a directory: {targetDir}\n\n", isError: true);
            }

            // TODO: hook up to debug flag
            /*
            var node = dir;
            var chain = new List<string>();
            while (node != null)
            {
                chain.Add(node.Name);
                node = node.Parent;
            }
            chain.Reverse();
            Console.WriteLine($"[DEBUG][LS] Listed directory parent chain: /{string.Join("/", chain)}");
            */

            if (dir.Children.Count == 0)
                return new CommandResult(string.Empty);

            // List dirs first, then files (alpha order)
            var dirs = new List<string>();
            var files = new List<string>();
            foreach (var child in dir.Children.Values)
            {
                if (child.IsDirectory)
                    dirs.Add(child.Name);
                else
                    files.Add(child.Name);
            }

            dirs.Sort(StringComparer.OrdinalIgnoreCase);
            files.Sort(StringComparer.OrdinalIgnoreCase);

            var output = string.Join("  ", dirs.Concat(files));
            return new CommandResult(output);
        }
    }
}
