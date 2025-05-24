using System;
using System.Collections.Generic;
using Env0.Terminal.Terminal;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;
using Env0.Terminal.Config;

namespace Env0.Terminal.Login
{
    public class SSHHandler
    {
        private const int MaxSshDepth = 10;
        private readonly NetworkManager _networkManager;

        public SSHHandler(NetworkManager networkManager)
        {
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
        }

        /// <summary>
        /// Attempts an SSH login to a target device.
        /// </summary>
        public (bool Success, string Output) StartSshSession(
            SessionState session,
            string targetHostOrIp,
            string providedUsername,
            Func<string, string> promptUserInput
        )
        {
            // Stack overflow check
            if (session.SshStack.Count >= MaxSshDepth)
            {
                session.SshStack.Clear();
                return (false, "Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.\n");
            }

            // Lookup device
            var device = _networkManager.FindDevice(targetHostOrIp);

            if (device == null || device.Ports == null || !device.Ports.Contains("22"))
                return (false, $"ssh: connect to host {targetHostOrIp} port 22: Connection refused\n");

            // Username logic
            string username = providedUsername;
            if (string.IsNullOrWhiteSpace(username))
            {
                username = promptUserInput != null ? promptUserInput("Username: ") : null;
                if (string.IsNullOrWhiteSpace(username))
                    return (false, "Login failed\n");
            }

            // Password prompt (hidden)
            string password = promptUserInput != null ? promptUserInput("Password: (input hidden)") : "";

            // Check credentials (case-sensitive)
            if (!string.Equals(device.Username, username) ||
                !string.Equals(device.Password, password))
            {
                // You can add a random delay here to mimic SSH if desired.
                return (false, "Login failed\n");
            }

            // Push current session context for SSH hop
            session.SshStack.Push(new SshSessionContext(
                session.Username,
                session.Hostname,
                session.CurrentWorkingDirectory,
                session.FilesystemManager,
                session.NetworkManager
            ));

            // Switch session context to the SSH target
            session.Username = username;
            session.Hostname = device.Hostname;
            session.CurrentWorkingDirectory = "/";
            session.FilesystemManager = LoadFilesystemForDevice(device);
            session.NetworkManager = _networkManager; // stays same in single-subnet
            session.DeviceInfo = device;

            // Show MOTD
            string motd = !string.IsNullOrWhiteSpace(device.Motd)
                ? device.Motd
                : $"Connected to {device.Hostname} ({device.Ip})";

            return (true, motd + "\n");
        }

        /// <summary>
        /// Exits an SSH session (if any).
        /// </summary>
        public (bool Success, string Output) ExitSsh(SessionState session)
        {
            if (session.SshStack.Count == 0)
                return (false, "You are already at the local terminal.\n");

            var prev = session.SshStack.Pop();
            session.Username = prev.Username;
            session.Hostname = prev.Hostname;
            session.CurrentWorkingDirectory = prev.CurrentWorkingDirectory;
            session.FilesystemManager = prev.FilesystemManager;
            session.NetworkManager = prev.NetworkManager;

            return (true, $"Connection to {session.Hostname} closed.\n");
        }

        /// <summary>
        /// Loads a FilesystemManager for the given device, using POCO-to-runtime conversion.
        /// </summary>
        private FilesystemManager LoadFilesystemForDevice(DeviceInfo device)
        {
            // Attempt main FS, then failsafe, else empty root
            if (!JsonLoader.Filesystems.TryGetValue(device.Filesystem, out var fsPoco) || fsPoco == null || fsPoco.Root == null)
            {
                if (!JsonLoader.Filesystems.TryGetValue("Filesystem_11.json", out fsPoco) || fsPoco == null || fsPoco.Root == null)
                {
                    var emptyRoot = new FileEntry
                    {
                        Type = "",
                        Content = null,
                        Children = new Dictionary<string, FileEntry>()
                    };
                    var emptyFsEntry = FileEntryToFileSystemEntryConverter.Convert("", emptyRoot, null);
                    return new FilesystemManager(emptyFsEntry);
                }
            }

            // Wrap POCO root in a FileEntry node, convert to FileEntry tree for runtime
            var pseudoRoot = new FileEntry
            {
                Type = "",
                Content = null,
                Children = fsPoco.Root
            };
            var fsEntryRoot = FileEntryToFileSystemEntryConverter.Convert("", pseudoRoot, null);

            return new FilesystemManager(fsEntryRoot);
        }

    }
}
