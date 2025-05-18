namespace env0.terminal.Terminal.Commands
{
    public class NmapCommand : CommandHandler
    {
        public override string Execute(string[] args)
        {
            // Simulate an output like `nmap` would give
            return "Device1  10.10.10.1\nDevice2  10.10.10.2";
        }
    }
}
