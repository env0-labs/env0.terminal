using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Config;

namespace Env0.Terminal.Terminal.Commands
{
    public class SshCommand : ICommand
    {
        public CommandResult Execute(SessionState session, string[] args)
        {
            if (session == null || session.NetworkManager == null)
                return new CommandResult("ssh: Network not initialized.\n", isError: true);

            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
                return new CommandResult("ssh: Missing target (hostname or IP).\n", isError: true);

            // Parse username@host if supplied, else just use host
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

            var device = session.NetworkManager.FindDevice(host);
            if (device == null)
                return new CommandResult($"ssh: Could not resolve host: {host}\n", isError: true);

            // Use device.Username and device.Password from Devices.json
            string expectedUser = device.Username ?? "admin";
            string expectedPass = device.Password ?? "password";

            if (user == null) user = expectedUser;

            if (user != expectedUser)
                return new CommandResult($"ssh: Incorrect username for {host}\n", isError: true);

            // Simulate password prompt (stub: always correct for now)
            string providedPass = expectedPass; // TODO: Replace with real user input

            if (providedPass != expectedPass)
                return new CommandResult($"ssh: Incorrect password for {user}@{host}\n", isError: true);

            // Check SSH stack depth
            if (session.SshStack.Count >= 10)
                return new CommandResult("ssh: Too many nested SSH sessions (max 10)\n", isError: true);

            // Push current session context onto stack
            session.SshStack.Push(new SshSessionContext(
                session.Username,
                session.Hostname,
                session.CurrentWorkingDirectory,
                session.FilesystemManager,
                session.NetworkManager
            ));

            // --- FilesystemManager assignment (using FileEntryToFileSystemEntryConverter) ---

            var fsFilename = device.Filesystem;
            if (!JsonLoader.Filesystems.TryGetValue(fsFilename, out var filesystem) || filesystem?.Root == null)
            {
                return new CommandResult($"ssh: Could not load filesystem '{fsFilename}' for device.\n", isError: true);
            }

            // Create root FileSystemEntry ("/") and convert all children using the new converter
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

            string banner = device.Motd ?? $"Connected to {device.Hostname}\n";
            return new CommandResult($"SSH connection established to {host}.\n{banner}", isError: false, stateChanged: true, updatedSession: session);
        }
    }
}
