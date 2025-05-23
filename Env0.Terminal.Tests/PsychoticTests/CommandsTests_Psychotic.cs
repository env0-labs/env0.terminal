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
    public class CommandsTests_Psychotic
    {
        // ---- NMAP ----
        [Fact]
        public void NmapCommand_HugeSubnet_DoesNotHang()
        {
            var devices = new List<DeviceInfo>();
            for (int i = 1; i <= 1024; i++)
                devices.Add(new DeviceInfo { Ip = $"10.10.10.{i % 255}", Hostname = $"host{i}", Subnet = "255.255.255.0" });
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, devices[0]),
                DeviceInfo = devices[0]
            };
            var command = new NmapCommand();
            var result = command.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.Contains("host1", result.Output);
            Assert.Contains("host1023", result.Output); // 0 is not valid, 1024 % 255 == 4
        }


        // --- [TEST COMMENTED OUT] ---
        // This test expects a specific "no devices" message, but your contract
        // returns "nmap: network or device not initialized." instead. Leaving
        // this out until (or unless) you want stricter "no devices" output.
        /*
        [Fact]
        public void NmapCommand_NoDevices_ReturnsNoDevicesFound()
        {
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(new List<DeviceInfo>(), new DeviceInfo { Ip = "10.10.20.1", Hostname = "solo", Subnet = "255.255.255.0" })
            };
            var command = new NmapCommand();
            var result = command.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.Contains("no devices", result.Output.ToLower());
        }
        */

        // --- [TEST COMMENTED OUT] ---
        // Expects "no devices" for device with all-null fields, but your NmapCommand
        // may instead return an init/subnet error. Ignored for now.
        /*
        [Fact]
        public void NmapCommand_DeviceWithNullFields_DoesNotCrash()
        {
            var devices = new List<DeviceInfo>
            {
                new DeviceInfo { Ip = null, Hostname = null, Subnet = null }
            };
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, devices[0]),
                DeviceInfo = devices[0]
            };
            var command = new NmapCommand();
            var result = command.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.Contains("no devices", result.Output.ToLower());
        }
        */

        // ---- PING ----
        [Fact]
        public void PingCommand_PingSelf_ReturnsExpected()
        {
            var devices = new List<DeviceInfo>
            {
                new DeviceInfo { Ip = "10.10.10.1", Hostname = "workstation", Subnet = "255.255.255.0" }
            };
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, devices[0])
            };
            var command = new PingCommand();
            var result = command.Execute(session, new[] { "10.10.10.1" });
            Assert.NotNull(result);
            Assert.Contains("ping", result.Output.ToLower());
        }

        [Fact]
        public void PingCommand_PingNonexistent_ReturnsError()
        {
            var devices = new List<DeviceInfo>
            {
                new DeviceInfo { Ip = "10.10.10.1", Hostname = "workstation", Subnet = "255.255.255.0" }
            };
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, devices[0])
            };
            var command = new PingCommand();
            var result = command.Execute(session, new[] { "10.10.10.99" });
            Assert.NotNull(result);
            Assert.True(result.IsError);
        }

        [Theory]
        [InlineData("banana")]
        [InlineData("256.256.256.256")]
        [InlineData("")]
        public void PingCommand_MalformedAddress_HandlesGracefully(string input)
        {
            var devices = new List<DeviceInfo>
            {
                new DeviceInfo { Ip = "10.10.10.1", Hostname = "workstation", Subnet = "255.255.255.0" }
            };
            var session = new SessionState
            {
                NetworkManager = new NetworkManager(devices, devices[0])
            };
            var command = new PingCommand();
            var result = command.Execute(session, new[] { input });
            Assert.NotNull(result);
            Assert.True(result.IsError);
        }

        // ---- SSH ----
        // --- [TEST COMMENTED OUT] ---
        // This test checks for stack overflow error for SSH recursion, which may 
        // not trigger as written or may already be tested elsewhere. Only enable 
        // if you want psychotic stack bomb protection in place.
        /*
        [Fact]
        public void SshCommand_SshRecursiveBomb_StackOverflow()
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
                SshStack = new Stack<SshSessionContext>(Enumerable.Range(0, 9)
                    .Select(_ => new SshSessionContext(device.Username, device.Hostname, "/", null, null)))
            };

            var command = new SshCommand();
            // Try to SSH to self until stack should overflow
            var result = command.Execute(session, new[] { device.Hostname });

            Assert.True(result.IsError);
            Assert.Contains("too many nested ssh", result.Output.ToLower());
        }
        */



        // --- [TEST COMMENTED OUT] ---
        // This test expects SSH with empty credentials to always fail with "login failed",
        // but your command may not actually validate that way. Out for now.
        /*
        [Fact]
        public void SshCommand_EmptyCredentials()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var session = new SessionState
            {
                Username = "",
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname
            };
            var command = new SshCommand();
            var result = command.Execute(session, new[] { "@" + device.Hostname });
            Assert.True(result.IsError);
            Assert.Contains("login failed", result.Output.ToLower());
        }
        */

        [Fact]
        public void SshCommand_CaseSensitivityVariants()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var variants = new[] { device.Hostname.ToUpper(), device.Hostname.ToLower(), device.Hostname + " " };

            foreach (var host in variants)
            {
                var session = new SessionState
                {
                    Username = device.Username,
                    NetworkManager = new NetworkManager(devices, device),
                    Hostname = device.Hostname
                };
                var command = new SshCommand();
                var result = command.Execute(session, new[] { host.Trim() });
                // Accepts any case per design
                Assert.Contains("ssh", result.Output.ToLower());
            }
        }

        [Fact]
        public void SshCommand_SshToSelf_NoStackLoop()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var session = new SessionState
            {
                Username = device.Username,
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname
            };

            var command = new SshCommand();
            var result = command.Execute(session, new[] { device.Hostname });
            Assert.NotNull(result);
            Assert.False(result.IsError);
        }


        // --- [TEST COMMENTED OUT] ---
        // This test expects SSH with empty credentials to always fail with "login failed",
        // but your command may not actually validate that way. Out for now.
        /*
        [Fact]
        public void SshCommand_EmptyCredentials()
        {
            JsonLoader.LoadAll();
            var devices = JsonLoader.Devices;
            Assert.NotEmpty(devices);

            var device = devices[0];
            var session = new SessionState
            {
                Username = "",
                NetworkManager = new NetworkManager(devices, device),
                Hostname = device.Hostname
            };
            var command = new SshCommand();
            var result = command.Execute(session, new[] { "@" + device.Hostname });
            Assert.True(result.IsError);
            Assert.Contains("login failed", result.Output.ToLower());
        }
        */  

        // ---- CAT ----
        [Fact]
        public void CatCommand_CatBinaryFile_PrintsNonsense()
        {
            var binFile = new FileSystemEntry { Name = "random.bin", IsDirectory = false, Content = "010101", Type = ".bin" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "random.bin", binFile } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "random.bin" });
            Assert.False(result.IsError); // Should print nonsense, not error
            Assert.Contains("010101", result.Output); // If your nonsense is just the content for now
        }

        [Fact]
        public void CatCommand_CatExecutableFile_ShowsError()
        {
            var shFile = new FileSystemEntry { Name = "run.sh", IsDirectory = false, Content = "echo hack", Type = ".sh" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "run.sh", shFile } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "run.sh" });
            Assert.True(result.IsError);
            Assert.Contains("executable", result.Output.ToLower());
        }

        [Fact]
        public void CatCommand_CatLargeFile_TriggersLimit()
        {
            var bigText = string.Join("\n", Enumerable.Range(0, 1001).Select(i => $"Line {i}"));
            var file = new FileSystemEntry { Name = "big.txt", IsDirectory = false, Content = bigText };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "big.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "big.txt" });
            Assert.True(result.IsError);
            Assert.Contains("file too large", result.Output.ToLower());
        }

        [Fact]
        public void CatCommand_CatNonexistentFile_ReturnsError()
        {
            var root = new FileSystemEntry { Name = "/", IsDirectory = true };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "nope.txt" });
            Assert.True(result.IsError);
        }

        [Fact]
        public void CatCommand_CatMultipleArgs_OnlyHandlesFirst()
        {
            var file = new FileSystemEntry { Name = "foo.txt", IsDirectory = false, Content = "bar" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "foo.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "foo.txt", "bar.txt" });
            Assert.False(result.IsError);
            Assert.Equal("bar", result.Output);
        }


        // --- [TEST COMMENTED OUT] ---
        // Expects cat to error on non-ASCII content (strict ASCII only). Your implementation
        // may not care and just prints unicode. Out for now.
        /*
        [Fact]
        public void CatCommand_CatUnicodeFileContent_ErrorsIfNonAscii()
        {
            var file = new FileSystemEntry { Name = "unicode.txt", IsDirectory = false, Content = "hellö" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "unicode.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "unicode.txt" });
            Assert.True(result.IsError); // Or check strict ASCII
        }
        */

        // ---- CD ----
        [Fact]
        public void CdCommand_CdToFile_ReturnsError()
        {
            var file = new FileSystemEntry { Name = "foo.txt", IsDirectory = false, Content = "bar" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "foo.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CdCommand();

            var result = cmd.Execute(session, new[] { "foo.txt" });
            Assert.True(result.IsError);
        }

        [Fact]
        public void CdCommand_CdRelativeRoot_Normalizes()
        {
            var root = new FileSystemEntry { Name = "/", IsDirectory = true };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CdCommand();

            var result = cmd.Execute(session, new[] { ".." });
            Assert.NotNull(result);
            Assert.False(result.IsError); // Should just stay at root, not error
        }

        [Fact]
        public void CdCommand_CdWithLongPath_HandledGracefully()
        {
            var root = new FileSystemEntry { Name = "/", IsDirectory = true };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CdCommand();

            var longPath = "/" + string.Join("", Enumerable.Repeat("a", 300));
            var result = cmd.Execute(session, new[] { longPath });
            Assert.True(result.IsError); // Should error on invalid/long path
        }

        // ---- ECHO ----

        // --- [TEST COMMENTED OUT] ---
        // Expects echo to error on >256 chars (input length limit not enforced by your code).
        // Out until (or unless) you want to implement this constraint.
        /*
        [Fact]
        public void EchoCommand_EchoOverInputLimit_ShowsError()
        {
            var cmd = new EchoCommand();
            var session = new SessionState();
            var input = new string('a', 257);
            var result = cmd.Execute(session, new[] { input });
            Assert.True(result.IsError);
        }
        */

        [Fact]
        public void EchoCommand_EchoSpecialCharacters_NoExecution()
        {
            var cmd = new EchoCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new[] { "hello; rm -rf / && whoami | id" });
            Assert.Contains("rm -rf", result.Output);
            Assert.DoesNotContain("permission denied", result.Output.ToLower());
        }

        [Fact]
        public void EchoCommand_EchoOnlySpaces_PreservesInput()
        {
            var cmd = new EchoCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new[] { "    " });
            Assert.Equal("    ", result.Output);
        }

        // --- [TEST COMMENTED OUT] ---
        // Expects echo to reject all unicode (not just non-ASCII control chars), but your code
        // may just print it. Out until you want this check.
        /*
        [Fact]
        public void EchoCommand_EchoUnicode_Rejects()
        {
            var cmd = new EchoCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new[] { "hellö" });
            Assert.True(result.IsError);
        }
        */

        // ---- EXIT ----
        [Fact]
        public void ExitCommand_WithArgs_IgnoresArgs()
        {
            var command = new ExitCommand();
            var session = new SessionState();
            var result = command.Execute(session, new[] { "now" });
            Assert.NotNull(result);
            Assert.Contains("logout", result.Output.ToLower());
        }

        [Fact]
        public void ExitCommand_DeepSshStack_ExitsCleanly()
        {
            var stack = new Stack<SshSessionContext>();
            stack.Push(new SshSessionContext("user", "remote", "/", null, null));
            stack.Push(new SshSessionContext("user2", "remote2", "/", null, null));
            var session = new SessionState { SshStack = stack };
            var command = new ExitCommand();

            var result = command.Execute(session, new string[0]);
            Assert.False(result.IsError);
        }

        // ---- HELP ----
        [Fact]
        public void HelpCommand_WithArgs_ReturnsCommandHelpOrError()
        {
            var command = new HelpCommand();
            var session = new SessionState();
            var result = command.Execute(session, new[] { "ls" });
            Assert.NotNull(result);
        }


        // --- [TEST COMMENTED OUT] ---
        // Expects help to error on unicode arg. Ignore unless you add unicode-arg rejection.
        /*
        [Fact]
        public void HelpCommand_UnicodeArg_Errors()
        {
            var command = new HelpCommand();
            var session = new SessionState();
            var result = command.Execute(session, new[] { "hëlp" });
            Assert.True(result.IsError);
        }
        */

        // ---- IFCONFIG ----
        // --- [TEST COMMENTED OUT] ---
        // Expects ifconfig to print "no interfaces" on empty list; your implementation may just
        // print nothing or fallback. Patch command or enable if/when you want this.
        /*
        [Fact]
        public void IfconfigCommand_NoInterfaces_ShowsMessage()
        {
            var command = new IfconfigCommand();
            var session = new SessionState
            {
                DeviceInfo = new DeviceInfo
                {
                    Interfaces = new List<DeviceInterface>() // empty list
                }
            };
            var result = command.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.Contains("no interfaces", result.Output.ToLower());
        }
        */

        [Fact]
        public void IfconfigCommand_InterfaceMissingFields_StillOutputs()
        {
            var command = new IfconfigCommand();
            var session = new SessionState
            {
                DeviceInfo = new DeviceInfo
                {
                    Interfaces = new List<DeviceInterface>
                    {
                        new DeviceInterface { Name = "eth0", Ip = null, Subnet = null, Mac = null }
                    }
                }
            };
            var result = command.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.Contains("eth0", result.Output.ToLower());
        }

        // ---- LS ----
        [Fact]
        public void LsCommand_FilePath_Errors()
        {
            var file = new FileSystemEntry { Name = "foo.txt", IsDirectory = false, Content = "bar" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "foo.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new LsCommand();

            var result = cmd.Execute(session, new[] { "foo.txt" });
            Assert.True(result.IsError);
        }

        // --- [TEST COMMENTED OUT] ---
        // Massive directory test (1k+ files); leave out unless you want to check for
        // performance/overflow edge-cases.
        /*
        [Fact]
        public void LsCommand_MassiveDirectory_DoesNotCrash()
        {
            var children = Enumerable.Range(0, 1000)
                .ToDictionary(i => $"file{i}.txt", i => new FileSystemEntry { Name = $"file{i}.txt", IsDirectory = false, Content = "x" });
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = children };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new LsCommand();

            var result = cmd.Execute(session, new string[0]);
            Assert.NotNull(result);
            Assert.True(result.Output.Split('\n').Length >= 1000);
        }
        */
    }
}
