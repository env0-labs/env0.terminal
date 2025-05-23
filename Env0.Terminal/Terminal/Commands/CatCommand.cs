using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;

namespace Env0.Terminal.Terminal.Commands
{
    public class CatCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // Defensive: session and FilesystemManager must be present
            if (session?.FilesystemManager == null)
                return new CommandResult("bash: cat: Filesystem not initialized.\n\n", isError: true);

            // Argument required
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                return new CommandResult("bash: cat: No file specified.\n\n", isError: true);

            // Only one file at a time per contract (ignore any additional args)
            string target = args[0].Trim();
            string content, error;
            bool success = session.FilesystemManager.GetFileContent(target, out content, out error);

            if (!success)
                return new CommandResult($"bash: cat: {error}\n\n", isError: true);

            return new CommandResult(content);
        }
    }
}