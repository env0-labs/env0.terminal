using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class PingCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Simulate ping using NetworkManager, random delay/loss per contract.
            return new CommandResult("ping: Not implemented yet.\n", isError: true);
        }
    }
}