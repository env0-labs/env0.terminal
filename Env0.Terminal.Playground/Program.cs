using System;
using Env0.Terminal.Terminal;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Config;

class Program
{
    static void Main(string[] args)
    {
        // 1. Load all devices (and filesystems) at startup
        JsonLoader.LoadAll(); // <-- assumes you have a LoadAll() that populates Devices, Filesystems, etc.

        // 2. Select a start device (just pick the first one for now)
        var devices = JsonLoader.Devices;
        if (devices == null || devices.Count == 0)
        {
            Console.WriteLine("No devices found. Aborting.");
            return;
        }

        var startDevice = devices[0];

        // 3. Initialize NetworkManager and FilesystemManager for session
        var networkManager = new NetworkManager(devices, startDevice);

        // 4. Build FileSystemEntry tree for this device
        var fsFilename = startDevice.Filesystem;
        if (!JsonLoader.Filesystems.TryGetValue(fsFilename, out var fs) || fs?.Root == null)
        {
            Console.WriteLine($"Could not load filesystem '{fsFilename}' for starting device.");
            return;
        }

        var rootEntry = new FileSystemEntry
        {
            Name = "/",
            IsDirectory = true,
            Children = new System.Collections.Generic.Dictionary<string, FileSystemEntry>()
        };
        foreach (var kvp in fs.Root)
        {
            var child = FileEntryToFileSystemEntryConverter.Convert(kvp.Key, kvp.Value, rootEntry);
            rootEntry.Children.Add(kvp.Key, child);
        }

        Console.WriteLine($"Filesystem root initialized with {rootEntry.Children.Count} children:");
        foreach (var kvp in rootEntry.Children)
            Console.WriteLine($"  - {kvp.Key} (dir? {kvp.Value.IsDirectory})");

        
        var filesystemManager = new FilesystemManager(rootEntry);

        // 5. Create session state
        var session = new SessionState
        {
            Username = startDevice.Username ?? "player",
            Hostname = startDevice.Hostname,
            DeviceInfo = startDevice,
            FilesystemManager = filesystemManager,
            NetworkManager = networkManager,
            CurrentWorkingDirectory = "/"
        };

        var commandHandler = new CommandHandler(debugMode: false);

        // 6. Main loop: prompt, accept, execute, print output, update session
        Console.WriteLine($"Welcome to env0.terminal. Starting at {session.Hostname}. Type 'help' for commands.");

        while (true)
        {
            Console.Write($"{session.Username}@{session.Hostname}:{session.CurrentWorkingDirectory}$ ");
            var input = Console.ReadLine();
            if (input == null)
                break; // EOF/CTRL+D

            var result = commandHandler.Execute(input, session);

            // Print result output
            if (!string.IsNullOrEmpty(result.Output))
                Console.WriteLine(result.Output.TrimEnd());

            // Optional: show error in red
            if (result.IsError)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Output.TrimEnd());
                Console.ForegroundColor = oldColor;
            }

            // If session was updated (ssh/exit), update local session reference
            if (result.UpdatedSession != null)
                session = result.UpdatedSession;

            // TODO: Implement real "logout" (exit program) on exit at stack bottom, if desired
        }
    }
}
