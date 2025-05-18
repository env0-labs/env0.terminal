/env0.terminal.unity (Workspace Root)
├── /src
│   └── /env0.terminal
│       ├── TerminalEngine.cs    (Core public API/facade for all engine functions)
│       ├── /Terminal
│       │   ├── TerminalManager.cs
│       │   ├── TerminalStateManager.cs
│       │   ├── CommandParser.cs
│       │   └── /Commands
│       │       ├── CommandHandler.cs
│       │       ├── LsCommand.cs
│       │       ├── CdCommand.cs
│       │       ├── PingCommand.cs
│       │       └── NmapCommand.cs
│       ├── /Boot
│       │   ├── BootSequenceHandler.cs
│       │   └── BootConfig.json
│       ├── /Login
│       │   ├── LoginHandler.cs
│       │   ├── UserManager.cs
│       │   └── UserConfig.json
│       ├── /Filesystem
│       │   ├── FileSystemManager.cs
│       │   ├── FileSystemLoader.cs
│       │   ├── FileSystemEntry.cs
│       │   └── /Files
│       │       ├── Filesystem_1.json
│       │       └── Filesystem_10.json
│       └── /Network
│           ├── NetworkManager.cs
│           ├── Devices.cs
│           ├── Device.cs
│           └── Devices.json
├── /config
│   ├── ConfigManager.cs
│   └── appsettings.json
└── /docs
    ├── README.md
    ├── DesignNotes.md
    └── TerminalContracts.md
