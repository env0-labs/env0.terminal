using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Config;
using System;

namespace Env0.Terminal.Terminal.Commands
{
    public class SshCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            // 1. Session/network validation
            if (session == null || session.NetworkManager == null)
                return new CommandResult("bash: ssh: Network not initialized.\n\n", isError: true);

            // 2. Argument validation (missing host)
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                return new CommandResult("bash: ssh: Missing host\n\n", isError: true);

            // 3. Parse user@host
            string user = null, host = null;
            var target = args[0].Trim();
            if (target.Contains("@"))
            {
                var split = target.Split('@');
                user = split[0];
                host = split[1];
            }
            else
            {
                host = target;
            }

            // 4. Device lookup
            var device = session.NetworkManager.FindDevice(host);
            if (device == null)
                return new CommandResult("bash: ssh: No such device\n\n", isError: true);

            // 5. Grab expected username/password from device (contract: only one user per device)
            string expectedUser = device.Username ?? "admin";
            string expectedPass = device.Password ?? "password";
            if (user == null) user = expectedUser;

            // 6. Contract: All SSH login failures return "Login failed" (never specify why)
            if (!string.Equals(user, expectedUser, StringComparison.OrdinalIgnoreCase))
                return new CommandResult("Login failed\n\n", isError: true);

            // --- Password check is stubbed for now ---
            // TODO: Replace this with real password prompt/user input when front-end allows
            string providedPass = expectedPass; // Always succeeds (for test only)

            if (providedPass != expectedPass)
                return new CommandResult("Login failed\n\n", isError: true);

            // 7. SSH stack overflow
            if (session.SshStack.Count >= 10)
                return new CommandResult("Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.\n\n", isError: true);

            // 8. Save current context on stack
            session.SshStack.Push(new SshSessionContext(
                session.Username,
                session.Hostname,
                session.CurrentWorkingDirectory,
                session.FilesystemManager,
                session.NetworkManager
            ));

            // 9. Filesystem loading
            var fsFilename = device.Filesystem;
            if (!JsonLoader.Filesystems.TryGetValue(fsFilename, out var filesystem) || filesystem?.Root == null)
                return new CommandResult($"bash: ssh: Could not load filesystem '{fsFilename}' for device.\n\n", isError: true);

            // 10. Build new root FS
            var rootEntry = new FileSystemEntry
            {
                Name = "/",
                IsDirectory = true,
                Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>()
            };

            foreach (var kvp in filesystem.Root)
            {
                var child = FileEntryToFileSystemEntryConverter.Convert(kvp.Key, kvp.Value, rootEntry);
                rootEntry.Children.Add(kvp.Key, child);
            }

            session.FilesystemManager = new FilesystemManager(rootEntry);
            session.NetworkManager.CurrentDevice = device;
            session.Username = user;
            session.Hostname = device.Hostname;
            session.DeviceInfo = device;
            session.CurrentWorkingDirectory = "/"; // Reset to root for new session

            // 11. Banner/MOTD (contract: always shown)
            string banner = !string.IsNullOrWhiteSpace(device.Motd)
                ? device.Motd
                : $"Welcome to {device.Hostname}";
            return new CommandResult(
                $"SSH connection established to {host}.\n{banner}\n",
                isError: false,
                stateChanged: true,
                updatedSession: session
            );
        }
    }
}
