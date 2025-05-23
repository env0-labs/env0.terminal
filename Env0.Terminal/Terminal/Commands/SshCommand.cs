using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class SshCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: SSH to another device. Parse target, invoke SSHHandler, etc.
            return new CommandResult("ssh: Not implemented yet.\n", isError: true);
        }
    }
}