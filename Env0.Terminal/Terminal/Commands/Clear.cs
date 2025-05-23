using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class ClearCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // This would normally clear terminal output—UI/frontend action only.
            return new CommandResult(string.Empty); // Output is empty to signal "clear"
        }
    }
}