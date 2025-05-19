# Task List / Project Progress — env0.terminal.unity

This file tracks all major system milestones and features as they are completed.
Update this list every time a new feature is finished and covered by tests.

---

## ✅ Completed

- [x] **Project structure:** Solution and project split (core, tests, playground)
- [x] **.gitignore:** Clean ignore rules for all platforms and build tools
- [x] **TerminalStateManager:** Core state machine (boot, login, shell, SSH)
- [x] **CommandParser:** Command parsing and normalization (case-insensitive, dangerous char filtering)
- [x] **FilesystemManager:** Full read-only virtual FS with:
    - [x] Directory listing (`ls`)
    - [x] Directory navigation (`cd`)
    - [x] Parent directory traversal (`cd ..`)
    - [x] Absolute path navigation (`cd /path`)
    - [x] Case-insensitive lookup for all files and directories
    - [x] File content read (`cat`)
    - [x] Error/edge handling for invalid/empty/missing files, directory/file confusion
- [x] **Test coverage:** 20/20 core tests, including adversarial and edge cases
- [x] **Playground console app:** Manual “smoke test” for navigation and file reading

---

## 🔜 In Progress / Next Up

- [ ] More adversarial and edge-case tests for FilesystemManager
- [ ] FilesystemManager: Support for empty directories, invalid names, directory/file name clashes
- [ ] FilesystemManager: Loading from JSON (future, when needed)
- [ ] LoginHandler: Local and SSH login with user/session state
- [ ] CommandHandler: Command dispatch and error output
- [ ] Integration: Connect all systems for real “terminal” flow

---

## 📝 Notes

- Update this file **every time you commit a major feature or pass a test milestone**
- Use as a single source of truth for migrating to new chat sessions or onboarding contributors

