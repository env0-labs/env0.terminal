using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class IfconfigCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: List all interfaces for current device.
            return new CommandResult("ifconfig: Not implemented yet.\n", isError: true);
        }
    }
}