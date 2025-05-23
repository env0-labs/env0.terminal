using System.Linq;
using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class NmapCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session?.NetworkManager == null || session.DeviceInfo == null)
                return new CommandResult("nmap: network or device not initialized.\n", isError: true);

            var subnet = session.DeviceInfo.Subnet;
            if (string.IsNullOrWhiteSpace(subnet))
                return new CommandResult("nmap: subnet not defined for current device.\n", isError: true);

            var devices = session.NetworkManager.GetDevicesOnSubnet(subnet);

            // Patch: Contract-compliant message if no devices
            if (devices == null || devices.Count == 0)
                return new CommandResult("No devices found on the subnet.\n");

            var output = string.Join("\n", devices.Select(d =>
                $"{d.Hostname ?? d.Ip} ({d.Ip})"));

            return new CommandResult(output);
        }
    }
}