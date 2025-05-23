using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;
using Env0.Terminal.Network;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Config;
using System.Collections.Generic;
using System.Linq;

namespace Env0.Terminal.Tests.Commands
{
    public class SshCommandTests
    {
        [Fact]
        public void SshCommand_ConnectsToDeviceAndUpdatesSession()
        {
            // Load all JSON config
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotNull(devices);
            Assert.NotEmpty(devices); // Fail early if loader isn't working

            var device = devices[0];

            // Build a valid session with correct user and device
            var session = new SessionState
            {
                Username = device.Username,
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname
            };

            var command = new SshCommand();

            // Try SSH by hostname
            var result = command.Execute(session, new[] { device.Hostname });

            Assert.NotNull(result);
            Assert.False(result.IsError);
            Assert.Contains("ssh connection established", result.Output.ToLower());
            Assert.True(result.StateChanged);
            Assert.NotNull(result.UpdatedSession);
            Assert.Equal(device.Hostname, result.UpdatedSession.Hostname);
            Assert.Equal(device.Username, result.UpdatedSession.Username);
            Assert.Equal("/", result.UpdatedSession.CurrentWorkingDirectory);

            // Banner/MOTD check
            if (!string.IsNullOrWhiteSpace(device.Motd))
                Assert.Contains(device.Motd.ToLower(), result.Output.ToLower());
        }

        [Fact]
        public void SshCommand_FailsOnUnknownHost()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var session = new SessionState
            {
                Username = "admin",
                NetworkManager = new NetworkManager(devices, devices[0]),
                Hostname = devices[0].Hostname
            };

            var command = new SshCommand();
            var result = command.Execute(session, new[] { "nonexistent-device" });

            Assert.True(result.IsError);
            // NEW: match canonical error string
            Assert.Contains("no such device", result.Output.ToLower());
        }

        [Fact]
        public void SshCommand_FailsOnIncorrectUsername()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var session = new SessionState
            {
                Username = "wronguser",
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname
            };

            var command = new SshCommand();
            var result = command.Execute(session, new[] { $"wronguser@{device.Hostname}" });

            Assert.True(result.IsError);
            // NEW: match canonical error string
            Assert.Contains("login failed", result.Output.ToLower());
        }

        [Fact]
        public void SshCommand_StackOverflowOnTooManyHops()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var session = new SessionState
            {
                Username = device.Username,
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname,
                SshStack = new Stack<SshSessionContext>(Enumerable.Range(0, 10)
                    .Select(_ => new SshSessionContext("user", "host", "/", null, null)))
            };

            var command = new SshCommand();
            var result = command.Execute(session, new[] { device.Hostname });

            Assert.True(result.IsError);
            Assert.Contains("too many nested ssh sessions", result.Output.ToLower());
        }
    }
}
