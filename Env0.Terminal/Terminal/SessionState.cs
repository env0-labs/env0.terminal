using System.Collections.Generic;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;

namespace Env0.Terminal.Terminal
{
    // Holds all mutable state for the current terminal session.
    public class SessionState
    {
        // Local user info (assigned at startup, never validated)
        public string Username { get; set; }
        public string Password { get; set; } // flavor only, not validated

        // Device context
        public string Hostname { get; set; } // Current device hostname
        public string Domain { get; set; }   // Game/session domain

        // Working directory (for the current session/device)
        public string CurrentWorkingDirectory { get; set; }

        // Managers
        public FilesystemManager FilesystemManager { get; set; }
        public NetworkManager NetworkManager { get; set; }

        // Command history (max 20, no consecutive duplicates)
        public List<string> CommandHistory { get; set; } = new List<string>();

        // SSH stack (for SSH hopping, max 10 as per Reference.md)
        public Stack<SshSessionContext> SshStack { get; set; } = new Stack<SshSessionContext>();

        // Debug flag
        public bool DebugMode { get; set; } = false;

        // Constructor
        public SessionState()
        {
            // Initialize default state
            Username = "player";
            Password = string.Empty;
            Hostname = "sbc";
            Domain = "node.zero";
            CurrentWorkingDirectory = "/";
            CommandHistory = new List<string>();
            SshStack = new Stack<SshSessionContext>();
            DebugMode = false;
        }
    }

    // Used to track SSH session context for hopping (expand as needed)
    public class SshSessionContext
    {
        public string Username { get; set; }
        public string Hostname { get; set; }
        public string CurrentWorkingDirectory { get; set; }
        public FilesystemManager FilesystemManager { get; set; }
        public NetworkManager NetworkManager { get; set; }

        public SshSessionContext(string username, string hostname, string cwd,
            FilesystemManager fsManager, NetworkManager netManager)
        {
            Username = username;
            Hostname = hostname;
            CurrentWorkingDirectory = cwd;
            FilesystemManager = fsManager;
            NetworkManager = netManager;
        }
    }
}
