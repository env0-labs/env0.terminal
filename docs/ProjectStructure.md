/env0.terminal.unity (Workspace Root)
├── /src                    (Core Logic - Pure C#)
│   └── /env0.core          (Pure C# Logic Engine)
│       ├── /Terminal       (Terminal Logic)
│       │   ├── TerminalManager.cs
│       │   ├── TerminalStateManager.cs
│       │   ├── CommandParser.cs
│       │   └── /Commands   (Modular Commands)
│       │       ├── CommandHandler.cs
│       │       ├── LsCommand.cs
│       │       ├── CdCommand.cs
│       │       ├── PingCommand.cs
│       │       └── NmapCommand.cs
│       │
│       ├── /Boot           (Boot Sequence)
│       │   ├── BootSequenceHandler.cs
│       │   └── BootConfig.json
│       │
│       ├── /Login          (User Login System)
│       │   ├── LoginHandler.cs
│       │   ├── UserManager.cs
│       │   └── UserConfig.json
│       │
│       ├── /Filesystem     (Virtual Filesystem Management)
│       │   ├── FileSystemManager.cs
│       │   ├── FileSystemLoader.cs
│       │   ├── FileSystemEntry.cs
│       │   └── /Files      (Filesystem JSONs)
│       │       ├── Filesystem_1.json
│       │       └── Filesystem_10.json
│       │
│       └── /Network        (Simulated Network)
│           ├── NetworkManager.cs
│           ├── Devices.cs
│           ├── Device.cs
│           └── Devices.json
│
├── /config                 (Central Config Files)
│   ├── ConfigManager.cs    (Manages JSON paths and global settings)
│   └── appsettings.json    (Global settings - debug, display options, etc.)
│
└── /docs                   (Documentation and Design Notes)
    ├── README.md
    ├── DesignNotes.md
    └── TerminalContracts.md
