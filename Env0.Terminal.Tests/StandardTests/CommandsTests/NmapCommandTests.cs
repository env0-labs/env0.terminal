using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;
using Env0.Terminal.Network;
using Env0.Terminal.Config.Pocos;
using System.Collections.Generic;

namespace Env0.Terminal.Tests.Commands
{
    public class NmapCommandTests
    {
        [Fact]
        public void NmapCommand_ListsDevicesOnSubnet()
        {
            var devices = new List<DeviceInfo>
            {
                new DeviceInfo { Ip = "10.10.10.1", Hostname = "workstation", Subnet = "255.255.255.0" }
            };
            var deviceInfo = devices[0];
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, deviceInfo),
                DeviceInfo = deviceInfo // <<<< KEY FIX
            };

            var command = new NmapCommand();
            var result = command.Execute(session, new string[0]);

            Assert.NotNull(result);
            Assert.Contains("workstation", result.Output);
        }

    }
}