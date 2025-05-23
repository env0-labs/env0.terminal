using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class PingCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session?.NetworkManager == null)
                return new CommandResult("ping: Network not initialized.\n", isError: true);

            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                return new CommandResult("ping: Missing hostname or IP.\n", isError: true);

            string target = args[0].Trim();
            var device = session.NetworkManager.FindDevice(target);

            if (device == null)
                return new CommandResult($"ping: {target}: Host unreachable\n", isError: true);

            // Optional: parse count from args (e.g., "ping -c 2 host")
            int packetCount = 4;
            if (args.Length > 2 && args[0] == "-c" && int.TryParse(args[1], out var c))
            {
                packetCount = c;
                target = args[2];
            }

            var results = session.NetworkManager.Ping(device, packetCount);
            var outputLines = new List<string>();
            foreach (var res in results)
            {
                if (res.Dropped)
                {
                    outputLines.Add($"Request timeout for icmp_seq {res.Sequence}");
                }
                else
                {
                    outputLines.Add($"Reply from {device.Ip}: icmp_seq={res.Sequence} ttl={res.Ttl} time={res.TimeMs} ms");
                }
            }
            outputLines.Add($"\n--- {device.Hostname} ping statistics ---");
            int received = results.Count(r => !r.Dropped);
            int lost = results.Count(r => r.Dropped);
            outputLines.Add($"{results.Count} packets transmitted, {received} received, {lost / (double)results.Count * 100:0.#}% packet loss");

            return new CommandResult(string.Join("\n", outputLines));
        }
    }
}