# env0.core – Project Milestones

## Milestone 1: Core Structure and Configuration (Foundation)
- Define and lock the modular directory structure (`Terminal`, `Boot`, `Login`, `Filesystem`, `Network`).
- Set up `ConfigManager` for centralized configuration:
  - JSON paths (`BootConfig.json`, `Filesystem.json`, `Devices.json`).
  - Debug settings (verbose logging, error handling style).
- Ensure `TerminalManager` and `TerminalStateManager` are cleanly separated:
  - `TerminalManager` handles text input/output.
  - `TerminalStateManager` handles state transitions (Boot, Login, Shell).
- Establish a clear, modular namespace structure:
  - `env0.core.Terminal`, `env0.core.Boot`, etc.

---

## Milestone 2: Boot and Login System (Core Experience)
- Build the `BootSequenceHandler` as a clean, modular system:
  - Loads boot lines from `BootConfig.json`.
  - Can be skipped with a key press.
  - Supports future expansion (visual effects, message of the day).
- Build the `LoginHandler` for local login:
  - Allows the player to set a username and password.
  - Loads user data from `UserConfig.json`.
  - Cleanly transitions to the Shell state on success.

---

## Milestone 3: Terminal and Command System (Core Interaction)
- Set up the `CommandParser` as a clean, modular command system.
- Commands are modular:
  - `CommandHandler.cs` (Base class).
  - Each command is its own file in `/Commands` (`LsCommand.cs`, `CdCommand.cs`).
- Build basic commands:
  - `ls`, `cd`, `clear`, `ping`, `nmap`, `read`, `cat`.
- Ensure error handling is consistent:
  - `"bash: [command]: [error message]"`

---

## Milestone 4: Filesystem (Virtual File Navigation)
- Build the `FileSystemManager` as a clean, JSON-driven virtual filesystem.
- Implement file and directory handling:
  - Files are JSON-defined (`Filesystem_1.json`).
  - Directories are dynamically built from JSON.
- Build basic file commands:
  - `ls`, `cd`, `mkdir`, `cat`, `read`, `touch`.
- Build `FileSystemLoader`:
  - Loads JSON files and builds the in-memory filesystem.

---

## Milestone 5: Network and Devices (Multi-System Simulation)
- Build the `NetworkManager` to manage simulated devices:
  - Load devices from `Devices.json`.
  - Each device has a unique IP, hostname, and filesystem type.
- Build SSH login as a separate system (not tied to local login).
- Implement network commands:
  - `ping` (sends a simulated packet).
  - `nmap` (lists all visible devices).
  - `ssh` (connects to a remote device).

---

## Milestone 6: Error Handling and Debugging (Polish)
- Build a consistent, Linux-style error system:
  - `"bash: [command]: [error message]"`
- Make all errors readable and helpful:
  - Invalid commands (`"bash: command not found"`).
  - Invalid paths (`"bash: cd: [path]: No such file or directory"`).
- Set up verbose logging for debugging (optional toggle in `appsettings.json`).

---

## Optional Milestones (Future Expansion)
- Build the AI Program (System AI vs. Cursed AI).
- Build Multi-User Support (LoginHandler can support multiple users).
- Add SSH Lockout (failed attempts cause lockout).
- Add Permissions System for Files (read, write, execute).
- Expand Network (Multi-subnet simulation).
