namespace env0.terminal.Terminal.Commands
{
    public abstract class CommandHandler
    {
        // Each command should implement this
        public abstract string Execute(string[] args);
    }
}
