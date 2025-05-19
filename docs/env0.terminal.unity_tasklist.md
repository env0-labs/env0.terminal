# Task List / Project Progress — env0.terminal.unity

Tracks all major milestones and system features.  
Update after each feature completion or green test run.

---

## ✅ Completed

- [x] **Project structure:**  
      Solution and project split (core, tests, playground)
- [x] **.gitignore:**  
      Clean ignore rules for all platforms/build tools
- [x] **TerminalStateManager:**  
      Core state machine (boot, login, shell, SSH)
- [x] **CommandParser:**  
      Command parsing and normalization (case-insensitive, dangerous char filtering)
- [x] **FilesystemManager:**  
      - Directory listing (`ls`)
      - Directory navigation (`cd`, `cd .`, `cd /path`)
      - Parent directory traversal
      - Absolute/relative path navigation
      - Case-insensitive lookup for files/directories
      - File content read (`cat`)
      - Error/edge handling: invalid/empty/missing files, directory/file confusion
      - Empty directories, empty files, large file (>1000 lines) error
      - File extension/type handling (.sh, .bin, unsupported, etc.)
      - Directory/file name validation (no dot, dot-dot, slashes, duplicates)
- [x] **Test coverage:**  
      - Standard suite for all core FS logic
      - **Hostile User Test Suite** for edge cases, invalid names, path traversal, collisions
- [x] **Playground console app:**  
      Live terminal for hands-on FS navigation and command execution
- [x] **JSON Loader:**  
      - `JsonLoader` implemented, supports BootConfig and future configs
      - Handles missing files, malformed JSON, empty values with clear errors
      - Full unit tests for loader (missing, malformed, empty, valid cases)
      - `InternalsVisibleTo` attribute implemented and confirmed

---

## 🔜 In Progress / Next Up

- [ ] **UserConfig, Devices, Filesystems:**  
      - Implement POCOs and extend `JsonLoader` for user/device/filesystem configs
      - Add edge-case/hostile tests for each
      - Validate cross-references (e.g., device references FS)
      - Fallback logic for missing or invalid config (safe-mode FS)
- [ ] LoginHandler:  
      Local and SSH login with user/session state
- [ ] CommandHandler:  
      Command dispatch and error output
- [ ] Integration:  
      Connect all systems for end-to-end terminal flow

---

## 📝 Notes

- Directory/file name validation:  
  - No empty, dot, dot-dot, or slashes  
  - No duplicates (case-insensitive)
- “Hostile user” edge cases are tested and passing
- JSON loader is now standard for all config loading/validation
- Update this file after each completed milestone or new test suite
