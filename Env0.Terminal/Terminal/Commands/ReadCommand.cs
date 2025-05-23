namespace Env0.Terminal.Terminal.Commands
{
    public class ReadCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Implement read logic
            return new CommandResult("Not implemented yet.\n", isError: true);
        }
    }
}