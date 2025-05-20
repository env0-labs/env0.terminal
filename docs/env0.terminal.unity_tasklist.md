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
    - [x] Directory navigation (`cd`, `cd ..`, `cd /path`)
    - [x] Parent directory traversal
    - [x] Absolute/relative path navigation
    - [x] Case-insensitive lookup for all files and directories
    - [x] File content read (`cat`)
    - [x] Error/edge handling for invalid/empty/missing files, directory/file confusion
    - [x] Empty directories, empty files, large file (>1000 lines) error
    - [x] File extension/type handling (.sh, .bin, unsupported, etc.)
    - [x] Directory/file name validation (no dot, dot-dot, slashes, or duplicates)
- [x] **BootConfig loader and validation:**
    - [x] BootConfig POCO
    - [x] Loader/parser integration with BootSequence
    - [x] Tests (standard and hostile: malformed, missing, out-of-spec)
    - [x] Debug output for validation errors
- [x] **Test coverage:**
    - [x] Standard test suite (20/20 green) for all core FS logic
    - [x] **Hostile User Test Suite** (`FilesystemManagerHostileUserTests.cs`):  
          - 27/27 “maniacs only” tests green  
          - Includes name collisions, path traversal, invalid names, edge-case validation, and more
- [x] **Playground console app:**  
      - Live interactive terminal for hands-on navigation and file access

---

## 🔜 In Progress / Next Up

- [ ] JSON loading and validation (Devices, UserConfig, Filesystem):
    - [ ] Loader must enforce all name/structure rules (already reflected in test)
    - [ ] Graceful fallback to safe-mode FS if JSON is corrupt/missing
- [ ] LoginHandler: Local and SSH login with user/session state
- [ ] CommandHandler: Command dispatch and error output
- [ ] Integration: Connect all systems for real “terminal” flow
- [ ] Future (horror/experimental branch):  
      - Recursive/cyclical folder logic
      - Cursed/haunted directory features

---

## 📝 Notes

- Directory/file name validation is now active:  
  - Names must not be empty, dot, dot-dot, or contain slashes
  - Adding a duplicate (case-insensitive) throws exception
- All “actively hostile user” edge cases now handled and tested
- FS manager is robust and future-proofed for future cursed logic experiments

- Update this file **every time you commit a major feature or pass a test milestone**
- Use as a single source of truth for migrating to new chat sessions or onboarding contributors
