# env0.terminal

A pure C# logic engine for simulating an authentic, modular Linux terminal.  
**No UI, no rendering, no Unity glue—just robust backend logic.**  
All front-end (Unity, CLI, etc.) interacts with this as a DLL or black-box API.

---

## 🚩 What is this?

**env0.terminal** is the core simulation for narrative, puzzle, or adventure games built on a Linux-style terminal experience.

- Implements authentic terminal navigation, strict file and network access, and a robust, case-insensitive command shell.
- All world data (filesystems, devices, boot sequence, users) is defined via JSON.
- Designed for integration into any front end via DLL—Unity, console, or other.

---

## 🛠️ Project Structure

- /
    - docs/
        - env0.terminal_Overview.md
        - env0.terminal_Q&A.txt
        - env0.terminal_REFERENCE.md
        - env0.terminal_tasklist.md
    - Env0.Terminal/
        - Config/
            - Jsons/
                - BootConfig.json
            - Pocos/
                - BootConfig.cs
            - JsonLoader.cs
            - AssemblyInfo.cs
        - Filesystem/
            - FilesystemManager.cs
        - Terminal/
            - CommandParser.cs
            - StateManager.cs
        - Class1.cs
        - Env0.Terminal.csproj
        - bin/...
        - obj/...
    - Env0.Terminal.Playground/
        - Env0.Terminal.Playground.csproj
        - Program.cs
        - bin/...
        - obj/...
    - Env0.Terminal.Tests/
        - CommandParserTests.cs
        - FilesystemManagerHostileUserTests.cs
        - FilesystemManagerTests.cs
        - JsonLoaderTests.cs
        - StateManagerTests.cs
        - Env0.Terminal.Tests.csproj
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
- **REFERENCE.md is law.** If you’re unsure, check [REFERENCE.md](./docs/env0.terminal.unity_REFERENCE.md)—it is always the source of truth.

---

## 📝 Development Cycle

The following cycle is strictly enforced for all core and feature work:
1. Decide the next feature or fix
2. Build the feature (code, logic, or config)
3. Build the test (unit/integration/hostile case as appropriate)
4. Test and debug until all automated tests pass
    - Includes standard and hostile/psychotic edge-case suites
    - Regularly interrogate and expand hostile test coverage
5. Modify for user test in Playground (manual verification, if relevant)
6. Commit once happy (atomic, meaningful commit; no “test/fix/test/fix” in main)
7. Update the Tasklist (env0.terminal.unity_tasklist.md)

This discipline is non-negotiable. All contributors and future development must adhere.

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

**For all implementation rules and edge cases, REFERENCE.md is canonical. When in doubt, check there or ask.**
