# env0.terminal.unity

A pure C# logic engine for simulating an authentic, modular Linux terminal.  
**No UI, no rendering, no Unity glue—just robust backend logic.**  
All front-end (Unity, CLI, etc.) interacts with this as a DLL or black-box API.

---

## 🚩 What is this?

**env0.terminal.unity** is the core simulation for narrative, puzzle, or adventure games built on a Linux-style terminal experience.

- Implements authentic terminal navigation, strict file and network access, and a robust, case-insensitive command shell.
- All world data (filesystems, devices, boot sequence, users) is defined via JSON.
- Designed for integration into any front end via DLL—Unity, console, or other.

---

## 🛠️ Project Structure

src/
└── env0.terminal
├── Boot/
│ ├── BootSequenceHandler.cs
│ └── BootConfig.json
├── Login/
│ ├── LoginHandler.cs
│ ├── UserManager.cs
│ └── UserConfig.json
├── Filesystem/
│ ├── FileSystemManager.cs
│ ├── FileSystemLoader.cs
│ ├── FileSystemEntry.cs
│ └── Files/
│ ├── Filesystem_1.json
│ └── Filesystem_11.json (safe mode)
├── Network/
│ ├── NetworkManager.cs
│ ├── Devices.cs
│ ├── Device.cs
│ └── Devices.json
├── Terminal/
│ ├── TerminalManager.cs
│ ├── TerminalStateManager.cs
│ └── CommandParser.cs
├── Commands/
│ ├── CommandHandler.cs
│ ├── LsCommand.cs
│ ├── CdCommand.cs
│ ├── CatCommand.cs
│ ├── ReadCommand.cs
│ ├── EchoCommand.cs
│ ├── PingCommand.cs
│ ├── NmapCommand.cs
│ ├── SshCommand.cs
│ ├── ClearCommand.cs
│ └── SudoCommand.cs
├── Tests/
│ ├── Standard test suite (xUnit)
│ └── Hostile edge case suite
└── Playground/
└── Interactive console app for hands-on testing


---

## 📚 Core Principles

- **Pure C# logic.** No UI, no direct Unity dependencies—strict separation of engine and presentation.
- **JSON-driven.** All world state, devices, users, filesystems, and boot text are defined in JSON.
- **Case-insensitive.** All commands, files, and directories are case-insensitive.
- **Read-only.** No file creation, editing, or deletion in Milestone 1.
- **Strict, Linux-like errors.** Predictable error handling, no hand-holding or auto-correction.
- **Session-only state.** No persistence; every run is a clean session unless handled by the consumer.
- **Debug mode is dev-only.** Toggled by command, never shipped in production builds.
- **RESET.md is law.** If you’re unsure, check [RESET.md](./env0.terminal.unity_RESET.md)—it is always the source of truth.

---

## 📝 Development Workflow

- Update [env0.terminal.unity_tasklist.md](./env0.terminal.unity_tasklist.md) after every major feature or test.
- All JSON schemas are validated at load—see RESET.md for failure/fallback rules.
- “Hostile User” test suite covers edge and adversarial cases.
- Front-end integration (Unity, CLI, etc.) is *not* included in this repo.

---

## ⚠️ Out of Scope

- No UI, rendering, or audio logic.
- No session saving/loading or cloud sync.
- No file creation or editing (read-only).
- No command chaining, tab-completion, or fuzzy matching.
- No environment variables or scripting.
- No multi-user or permission systems (yet).

---

## 📄 License

Copyright © Ewan Matheson  
MIT License 

---

**For all implementation rules and edge cases, RESET.md is canonical. When in doubt, check there or ask.**
