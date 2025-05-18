namespace env0.terminal.Terminal.Commands
{
    public class PingCommand : CommandHandler
    {
        public override string Execute(string[] args)
        {
            // Simulate a ping output
            return "Pinging 10.10.10.1 with 32 bytes of data: Reply from 10.10.10.1: bytes=32 time<1ms TTL=64";
        }
    }
}
