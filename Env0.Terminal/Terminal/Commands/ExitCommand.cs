using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class ExitCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Exit SSH session or print "You are already at the local terminal." if at root.
            return new CommandResult("exit: Not implemented yet.\n", isError: true);
        }
    }
}