using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;
using System;
using System.Collections.Generic;

namespace Env0.Terminal.Terminal.Commands
{
    public class CdCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session?.FilesystemManager == null)
                return new CommandResult("bash: cd: Filesystem not initialized.\n\n", isError: true);

            string targetDir = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? "/" // Or session.HomeDirectory
                : args[0];
            // If targetDir is ".", use current working directory (effectively a no-op)
            if (targetDir == ".")
                targetDir = session.CurrentWorkingDirectory;

            string error;
            bool success = session.FilesystemManager.ChangeDirectory(targetDir, out error);

            if (success)
            {
                // Update session with the new absolute path!
                session.CurrentWorkingDirectory = session.FilesystemManager.GetCurrentDirectoryPath();
                Console.WriteLine($"[DEBUG] cd success: now at {session.CurrentWorkingDirectory}");

                // Print the full parent chain for the current directory
                var node = session.FilesystemManager.CurrentDirectory; // Adjust this property name if needed
                var chain = new List<string>();
                while (node != null)
                {
                    chain.Add(node.Name);
                    node = node.Parent;
                }
                chain.Reverse();
                Console.WriteLine($"[DEBUG] CWD parent chain: /{string.Join("/", chain)}");

                return new CommandResult(string.Empty);
            }
            else
            {
                Console.WriteLine($"[DEBUG] cd failed: {error}");
                return new CommandResult($"bash: cd: {error}\n\n", isError: true);
            }
        }
    }
}
