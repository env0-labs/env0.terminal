# env0.terminal.unity

A pure C# logic engine for simulating an authentic, modular Linux terminal.  
**No UI, no rendering, no Unity glue—just robust backend logic.**  
All front-end (Unity, CLI, etc.) interacts with this as a DLL or black-box API.

---

## 🚩 What is this?

**env0.terminal.unity** is the core simulation for narrative, puzzle, or adventure games built on a Linux-style terminal experience.

- Implements strict Linux-style terminal navigation, file access, and SSH-based network traversal.
- All world data (filesystems, devices, boot sequences, users) is defined via JSON and fully session-driven.
- Built to be consumed as a DLL—Unity is one possible host, but the logic is fully standalone.

---

## ✅ Current Status

- All converters are unified under a single canonical FileEntry model.
- The filesystem tree is fully parent-linked and supports nested navigation with proper error recovery.
- SSH and command execution are stable across recursive and multi-hop sessions.
- Full test coverage exists across standard, hostile, and psychotic cases.
- Debug output exists throughout but is now cleanly commented and tagged for future toggling.

This is a known-good checkpoint.

---

## 🛠️ Project Structure

/
├── docs/
│ ├── env0.terminal.unity_Overview.md
│ ├── env0.terminal.unity_Q&A.txt
│ ├── env0.terminal.unity_REFERENCE.md
│ └── env0.terminal.unity_tasklist.md
├── Env0.Terminal/
│ ├── Config/
│ │ ├── Jsons/
│ │ │ └── BootConfig.json
│ │ ├── Pocos/
│ │ │ └── BootConfig.cs
│ │ └── JsonLoader.cs
│ ├── Filesystem/
│ │ └── FilesystemManager.cs
│ ├── Terminal/
│ │ ├── CommandParser.cs
│ │ ├── StateManager.cs
│ │ └── Commands/
│ │ ├── LsCommand.cs
│ │ ├── CdCommand.cs
│ │ ├── CatCommand.cs
│ │ ├── NmapCommand.cs
│ │ ├── ExitCommand.cs
│ │ ├── EchoCommand.cs
│ │ ├── SshCommand.cs
│ │ └── HelpCommand.cs
│ └── Env0.Terminal.csproj
├── Env0.Terminal.Playground/
│ ├── Program.cs
│ └── Env0.Terminal.Playground.csproj
├── Env0.Terminal.Tests/
│ ├── StandardTests/
│ ├── PsychoticTests/
│ ├── HostileUserTests/
│ └── Env0.Terminal.Tests.csproj
├── .gitignore
├── Env0.Terminal.sln
└── env0.terminal.unity.code-workspace


---

## 📚 Core Principles

- **Pure C# logic.** No UI, no Unity dependencies—strict backend engine.
- **JSON-driven.** All world content (devices, filesystems, users) is externally defined.
- **Case-insensitive.** Commands, files, and directories are all case-insensitive.
- **Read-only.** No file creation/editing in Milestone 1—filesystem is static per session.
- **Strict errors.** All command errors follow Linux conventions: no hand-holding, no suggestions.
- **Session-only state.** No persistence or mutation outside the runtime session.
- **Debug mode is dev-only.** Controlled by future flags—not exposed to players.
- **REFERENCE.md is law.** It governs system behavior and test expectations. If you’re unsure, read [REFERENCE.md](./docs/env0.terminal.unity_REFERENCE.md).

---

## 📝 Development Cycle

The following discipline is enforced for all code and content changes:

1. Decide the next feature or fix
2. Build the feature (code, logic, or config)
3. Build the test (unit/integration/hostile case)
4. Test and debug until all automated tests pass
   - Includes standard, hostile, and edge-case coverage
5. Modify for manual test in Playground (if needed)
6. Commit once happy (atomic, meaningful commit only)
7. Update the Tasklist (`env0.terminal.unity_tasklist.md`)

This process is non-negotiable. All contributors must adhere.

---

## ⚠️ Out of Scope

- No UI, rendering, or audio logic
- No persistent session state or save/load
- No fuzzy matching, command history, or autocomplete
- No permission system or multi-user logic (yet)
- No write access or script execution
- No environment variables or piping

---

## 📄 License

Copyright © Ewan Matheson  
MIT License

---

**For all implementation rules and edge cases, `REFERENCE.md` is canonical. When in doubt, check there or ask.**
