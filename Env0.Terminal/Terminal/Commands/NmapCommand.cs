using System;
using System.Linq;
using Env0.Terminal.Terminal;
using Env0.Terminal.Network;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Terminal.Commands
{
    public class NmapCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session?.NetworkManager == null || session.DeviceInfo == null)
                return new CommandResult("bash: nmap: network or device not initialized\n", isError: true);

            // No arguments: print bash error
            if (args.Length == 0)
                return new CommandResult("bash: nmap: target or subnet required\n");

            var target = args[0].Trim();

            // If target is CIDR (only /24 for now)
            if (IsValidCidr(target))
            {
                var devices = session.NetworkManager.GetDevicesOnSubnet(target);
                if (devices == null || devices.Count == 0)
                    return new CommandResult("No devices found on the subnet.\n");

                var output = string.Join("\n", devices.Select(d =>
                    $"{d.Ip,-15} {d.Hostname,-25} [Ports: {string.Join(", ", d.Ports)}] {d.Description}"
                ));
                return new CommandResult($"Starting Nmap scan for subnet {target}...\n\n{output}\n");
            }

            // Host/IP lookup
            var device = session.NetworkManager.FindDevice(target);
            if (device != null)
            {
                var output = $"{device.Ip,-15} {device.Hostname,-25} [Ports: {string.Join(", ", device.Ports)}] {device.Description}";
                return new CommandResult($"Starting Nmap scan for host {target}...\n\n{output}\n");
            }

            // If it looks like a subnet (contains /), but is not valid
            if (target.Contains("/"))
                return new CommandResult("bash: nmap: invalid subnet format\n");

            // Otherwise, host not found
            return new CommandResult("bash: nmap: host not found\n");
        }

        // Only /24 support for now
        private bool IsValidCidr(string input)
        {
            var parts = input.Split('/');
            if (parts.Length != 2) return false;
            int mask;
            return System.Net.IPAddress.TryParse(parts[0], out _) && int.TryParse(parts[1], out mask) && mask == 24;
        }
    }
}
