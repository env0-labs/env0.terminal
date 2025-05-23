using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class NmapCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Simulate nmap using NetworkManager, list devices on subnet.
            return new CommandResult("nmap: Not implemented yet.\n", isError: true);
        }
    }
}