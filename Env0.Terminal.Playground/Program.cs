using System;
using System.Linq;
using Env0.Terminal.Config;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Login;
using Env0.Terminal.Network;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;

namespace Env0.Terminal.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load all configs (devices, filesystems, etc.)
            JsonLoader.LoadAll();

            // Setup managers (find initial device)
            var initialDevice = JsonLoader.Devices.FirstOrDefault(d => d.Hostname == "workstation.node.zero");
            if (initialDevice == null)
            {
                Console.WriteLine("Error: Initial device 'workstation.node.zero' not found in Devices.json!");
                return;
            }
            var networkManager = new NetworkManager(JsonLoader.Devices, initialDevice);

            // Prepare FS root (wrap POCO root as directory, then convert to runtime type)
            var fsPoco = JsonLoader.Filesystems["Filesystem_1.json"];
            var pseudoRoot = new FileEntry
            {
                Type = "",
                Content = null,
                Children = fsPoco.Root
            };
            var rootEntry = FileEntryToFileSystemEntryConverter.Convert("", pseudoRoot, null);
            var fsManager = new FilesystemManager(rootEntry);


            // Initialize session state
            var session = new SessionState
            {
                Username = "alice",
                Hostname = "workstation.node.zero",
                DeviceInfo = initialDevice,
                FilesystemManager = fsManager,
                NetworkManager = networkManager
            };

            // TODO initial FS debug output - leaving in and commented for possible debug flag
            /*
            Console.WriteLine($"Filesystem root initialized with {rootEntry.Children.Count} children:");
            foreach (var kv in rootEntry.Children)
                Console.WriteLine($"  - {kv.Key} (dir? {kv.Value.Type == ""})");

            Console.WriteLine($"Welcome to env0.terminal. Starting at {session.Hostname}. Type 'help' for commands.");
            */

            var sshHandler = new SSHHandler(networkManager);
            var commandHandler = new CommandHandler(false);

            while (true)
            {
                Console.Write($"{session.Username}@{session.Hostname}:{session.CurrentWorkingDirectory}$ ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var tokens = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var command = tokens[0].ToLower();
                var arguments = tokens.Length > 1 ? tokens[1] : "";

                if (command == "exit")
                {
                    var (success, output) = sshHandler.ExitSsh(session);
                    Console.WriteLine(output);
                    continue;
                }

                if (command == "ssh")
                {
                    string userPart = null, hostPart = null;

                    // Parse ssh [user@]host
                    var sshArg = arguments.Trim();
                    if (sshArg.Contains("@"))
                    {
                        var parts = sshArg.Split('@');
                        userPart = parts[0].Trim();
                        hostPart = parts[1].Trim();
                    }
                    else
                    {
                        hostPart = sshArg;
                    }

                    var (success, output) = sshHandler.StartSshSession(
                        session,
                        hostPart,
                        userPart,
                        PromptUser // delegate for username/password input
                    );
                    Console.WriteLine(output);
                    continue;
                }

                if (command == "help")
                {
                    Console.WriteLine("Commands: ssh, exit, help, and all standard terminal commands (ls, cd, cat, nmap, etc.)");
                    continue;
                }

                // All other commands handled by your real handler!
                var result = commandHandler.Execute(input, session);
                if (!string.IsNullOrWhiteSpace(result.Output))
                    Console.WriteLine(result.Output);
            }
        }

        // Input prompt: username or password
        public static string PromptUser(string prompt)
        {
            Console.Write(prompt);
            if (prompt.ToLower().Contains("password"))
                return ReadPassword();
            return Console.ReadLine();
        }

        public static string ReadPassword()
        {
            var pwd = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                        pwd = pwd.Substring(0, pwd.Length - 1);
                    continue;
                }
                pwd += key.KeyChar;
            } while (true);
            Console.WriteLine();
            return pwd;
        }
    }
}
