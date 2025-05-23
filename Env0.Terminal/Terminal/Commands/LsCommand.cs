using Env0.Terminal.Filesystem;

namespace Env0.Terminal.Terminal.Commands
{
    public class LsCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            var fsManager = session.FilesystemManager;
            string targetDir = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? "." // Current dir
                : args[0];

            // Use the new helper!
            if (!fsManager.TryGetDirectory(targetDir, out var dir, out var error))
                return new CommandResult($"bash: ls: {error}\n\n", isError: true);

            if (!dir.IsDirectory)
                return new CommandResult($"bash: ls: Not a directory: {targetDir}\n\n", isError: true);

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