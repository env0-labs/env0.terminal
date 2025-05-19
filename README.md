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

- /
    - docs/
        - env0.terminal.unity_Overview.md
        - env0.terminal.unity_Q&A.txt
        - env0.terminal.unity_RESET.md
        - env0.terminal.unity_tasklist.md
    - Env0.Terminal/
        - Class1.cs
        - CommandParser.cs
        - Env0.Terminal.csproj
        - FilesystemManager.cs
        - StateManager.cs
        - bin/...
        - obj/...
    - Env0.Terminal.Playground/
        - Env0.Terminal.Playground.csproj
        - Program.cs
        - bin/...
        - obj/...
    - Env0.Terminal.Tests/
        - CommandParserTests.cs
        - Env0.Terminal.Tests.csproj
        - FilesystemManagerHostileUserTests.cs
        - FilesystemManagerTests.cs
        - StateManagerTests.cs
        - UnitTest1.cs
        - bin/...
        - obj/...
    - .gitignore
    - Env0.Terminal.sln
    - env0.terminal.unity.code-workspace



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
