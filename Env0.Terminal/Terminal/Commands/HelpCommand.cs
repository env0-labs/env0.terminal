using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class HelpCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Output comprehensive, paginated help per contract.
            return new CommandResult("help: Not implemented yet.\n", isError: true);
        }
    }
}