namespace env0.terminal.Terminal.Commands
{
    public class CdCommand : CommandHandler
    {
        public override string Execute(string[] args)
        {
            if (args.Length == 0)
            {
                return "bash: cd: missing operand";
            }

            // For now, just simulate moving to the directory
            return $"Changed directory to {args[0]}";
        }
    }
}
