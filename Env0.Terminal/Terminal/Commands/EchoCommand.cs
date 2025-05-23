namespace Env0.Terminal.Terminal.Commands
{
    public class EchoCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // TODO: Implement echo logic
            return new CommandResult("Not implemented yet.\n", isError: true);
        }
    }
    }