using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;

namespace Env0.Terminal.Terminal.Commands
{
    public class ExitCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session.SshStack != null && session.SshStack.Count > 0)
            {
                // Pop previous SSH context
                var prev = session.SshStack.Pop();

                session.Username = prev.Username;
                session.Hostname = prev.Hostname;
                session.CurrentWorkingDirectory = prev.CurrentWorkingDirectory;
                session.FilesystemManager = prev.FilesystemManager;
                session.NetworkManager = prev.NetworkManager;

                // Optionally update DeviceInfo as well if your model supports it

                return new CommandResult(
                    $"Connection to {session.Hostname} closed.\n",
                    isError: false,
                    stateChanged: true,
                    updatedSession: session
                );
            }
            else
            {
                // Not in SSH, could end session or just say nothing
                return new CommandResult("logout: Not implemented.\n", isError: true);
            }
        }
    }
}