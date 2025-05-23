using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class SudoCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // Always returns Easter egg per contract.
            return new CommandResult("Nice try.\n");
        }
    }
}