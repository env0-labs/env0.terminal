using Env0.Terminal.Filesystem;

namespace Env0.Terminal.Terminal.Commands
{
    public class CatCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Implement cat logic
            return new CommandResult("Not implemented yet.\n", isError: true);
        }
    }
}