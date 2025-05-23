using Env0.Terminal.Terminal;
using Env0.Terminal.Config.Pocos; // For DeviceInfo and NetworkInterfaceInfo

namespace Env0.Terminal.Terminal.Commands
{
    public class IfconfigCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // Try to get device info from session, else output a stub
            var device = session?.DeviceInfo;

            if (device == null || device.Interfaces == null || device.Interfaces.Count == 0)
            {
                // Fallback: output stub info if nothing real
                string stub = @"
eth0:  inet 10.10.10.99  netmask 255.255.255.0  mac aa:bb:cc:dd:ee:ff
lo:    inet 127.0.0.1     netmask 255.0.0.0
";
                return new CommandResult(stub.Trim());
            }

            // Otherwise, format actual interfaces from device info
            var lines = new List<string>();
            foreach (var iface in device.Interfaces)
            {
                lines.Add($"{iface.Name}:  inet {iface.Ip}  netmask {iface.Subnet}  mac {iface.Mac}");
            }
            return new CommandResult(string.Join("\n", lines));
        }
    }
}