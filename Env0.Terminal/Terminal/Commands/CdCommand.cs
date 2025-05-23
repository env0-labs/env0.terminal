using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;

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

            string error;
            bool success = session.FilesystemManager.ChangeDirectory(targetDir, out error);

            if (!success)
                return new CommandResult($"bash: cd: {error}\n\n", isError: true);

            // Update session with the new absolute path!
            session.CurrentWorkingDirectory = session.FilesystemManager.GetCurrentDirectoryPath();

            return new CommandResult(string.Empty);

        }

    }

    // Example of what ChangeDirectory might return (adjust as needed)
    public class ChangeDirectoryResult
    {
        public bool Success { get; set; }
        public string NewDirectory { get; set; }
        public string ErrorMessage { get; set; }
    }
}