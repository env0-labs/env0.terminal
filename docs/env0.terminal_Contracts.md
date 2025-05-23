# env0.terminal – Canonical Component Map (Source/Config Only)

| **File**                                       | **Purpose**                                                        | **Uses**                                 | **Used By**                                                      | **Completion** |
|------------------------------------------------|--------------------------------------------------------------------|------------------------------------------|------------------------------------------------------------------|:-------------:|
| Config/Jsons/Filesystems/Filesystem_1.json     | Virtual filesystem for Device #1                                   | (none)                                  | JsonLoader, FilesystemManager                                     |      ✅       |
| Config/Jsons/Filesystems/Filesystem_11.json    | Failsafe filesystem for error/fallback cases                       | (none)                                  | JsonLoader, FilesystemManager                                     |      ✅       |
| Config/Jsons/BootConfig.json                   | Boot sequence display text                                         | (none)                                  | JsonLoader, Boot logic                                            |      ✅       |
| Config/Jsons/Devices.json                      | Declares all network devices, user/pass, IPs, hostnames, FS refs   | (none)                                  | JsonLoader, NetworkManager                                        |      ✅       |
| Config/Jsons/UserConfig.json                   | Default/fallback user creds for tests/dev only                     | (none)                                  | JsonLoader, LoginHandler, Tests                                   |      ✅       |
| Config/Pocos/BootConfig.cs                     | POCO: Structure for BootConfig.json                                | (none)                                  | JsonLoader                                                        |      ✅       |
| Config/Pocos/Devices.cs                        | POCO: Structure for Devices.json                                   | (none)                                  | JsonLoader, NetworkManager                                        |      ✅       |
| Config/Pocos/FileEntry.cs                      | POCO: Node for file or directory in FS (name, type, content, etc.) | (none)                                  | Filesystem.cs, FilesystemManager                                  |      ✅       |
| Config/Pocos/FileEntryConverter.cs             | Handles JSON polymorphism (file vs dir nodes)                      | (none)                                  | JsonLoader, Filesystem.cs                                         |      ✅       |
| Config/Pocos/Filesystem.cs                     | POCO: Root structure for FS JSON (tree root, props)                | (none)                                  | JsonLoader, FilesystemManager                                     |      ✅       |
| Config/Pocos/UserConfig.cs                     | POCO: Structure for UserConfig.json                                | (none)                                  | JsonLoader, LoginHandler                                          |      ✅       |
| Config/AssemblyInfo.cs                         | Standard .NET assembly metadata                                    | (none)                                  | Build/runtime only                                                |      ✅       |
| Config/JsonLoader.cs                           | Loads, validates, and exposes all JSON config                      | All POCOs, System.IO                    | All managers/handlers needing config, esp. FilesystemManager      |      ✅       |
| Filesystem/FilesystemManager.cs                | Main interface for virtual FS: navigation, read, errors, etc.      | Filesystem.cs, FileEntry.cs, JsonLoader  | CommandHandler, SSHHandler, TerminalStateManager, LoginHandler   |      ✅       |
| Login/LoginHandler.cs                          | Handles username assignment (local) and SSH login                  | UserConfig.cs, Devices.cs, NetworkManager| TerminalStateManager, SSHHandler                                 |   🚧 (Stub)   |
| Login/LoginResultStatus.cs                     | Enum/status for login attempts (success/fail/etc)                  | (none)                                  | LoginHandler                                                      |   🚧 (Stub)   |
| Login/SSHHandler.cs                            | Manages SSH session stack and SSH-specific login                   | Devices.cs, FilesystemManager            | TerminalStateManager, CommandHandler                             |   🚧 (Stub)   |
| Network/NetworkManager.cs                      | Handles device lookups, nmap, ping, interface listing              | Devices.cs                               | CommandHandler, LoginHandler, SSHHandler                         |      ✅       |
| Terminal/CommandHandler.cs                     | Central command dispatcher; finds/executes ICommand (see below)    | ICommand, receives parsed command from CommandParser | TerminalStateManager, Playground/CLI                 |   🚧 (Not implemented – contract below) |
| Terminal/ICommand.cs                           | Common interface for all command modules (see below)               | (none)                                  | CommandHandler, all command classes                               |   🚧 (Not implemented – contract below) |
| Terminal/Commands/LsCommand.cs                 | Implements `ls` (list directory contents) command                    | FilesystemManager, SessionState             | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/CdCommand.cs                 | Implements `cd` (change directory) command                          | FilesystemManager, SessionState             | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/CatCommand.cs                | Implements `cat` (display file contents) command                    | FilesystemManager, SessionState             | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/ReadCommand.cs               | Implements `read` (paginated file reader) command                   | FilesystemManager, SessionState             | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/EchoCommand.cs               | Implements `echo` (output text) command                             | SessionState                                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/ClearCommand.cs              | Implements `clear` (clear terminal output) command                  | SessionState                                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/PingCommand.cs               | Implements `ping` (simulate network ping) command                   | NetworkManager, SessionState                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/NmapCommand.cs               | Implements `nmap` (scan/list devices on network) command            | NetworkManager, SessionState                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/SshCommand.cs                | Implements `ssh` (connect to remote device) command                 | NetworkManager, SSHHandler, LoginHandler, SessionState | CommandHandler, TerminalStateManager | ❌ (Not yet implemented) |
| Terminal/Commands/ExitCommand.cs               | Implements `exit` (exit SSH or shell) command                       | SSHHandler, SessionState                    | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/SudoCommand.cs               | Implements `sudo` (Easter egg: always returns "Nice try.") command  | SessionState                                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/HelpCommand.cs               | Implements `help` (list and describe commands) command              | Command registry, SessionState              | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/IfconfigCommand.cs           | Implements `ifconfig` (list network interfaces for current device)  | NetworkManager, SessionState                | CommandHandler, TerminalStateManager      | ❌ (Not yet implemented) |
| Terminal/Commands/ShowFilesystemsCommand.cs | Debug-only: lists all loaded filesystems                    | FilesystemManager, DebugManager, SessionState | CommandHandler (if debug enabled)      | ❌ (Not yet implemented) |
| Terminal/Commands/ListDevicesCommand.cs     | Debug-only: shows all devices, including hidden             | NetworkManager, DebugManager, SessionState    | CommandHandler (if debug enabled)      | ❌ (Not yet implemented) |
| Terminal/Commands/TeleportCommand.cs        | Debug-only: instantly moves to any directory (confirmation) | FilesystemManager, DebugManager, SessionState | CommandHandler (if debug enabled)      | ❌ (Not yet implemented) |
| Terminal/Commands/ClearDebugCommand.cs      | Debug-only: clears debug info/output                        | DebugManager, SessionState                    | CommandHandler (if debug enabled)      | ❌ (Not yet implemented) |
| Terminal/Commands/                             | Placeholder for future modular command logic                       | (none)                                  | (none yet)                                                        |   ❌ (Planned)|
| Terminal/CommandParser.cs                      | Parses/normalizes user input, strips illegal chars                 | (none)                                  | CommandHandler, TerminalStateManager                              |      ✅       |
| Terminal/TerminalStateManager.cs               | Controls all terminal session/SSH state, transitions, prompts      | All above managers/handlers              | Playground, user entry, other managers                           |      ✅       |
| Terminal/DebugManager.cs                       | Controls debug mode toggle, debug command enablement, and debug output sections; collects/logs debug info/errors from all managers | (none) | CommandHandler, TerminalStateManager, all managers needing debug flag or output   | 🚧 (Not implemented – contract below) |
| Terminal/SessionState.cs   | Stores all session data: username, password, device, cwd, history, SSH stack, debug flag, etc.           | (none)          | TerminalStateManager, CommandHandler, ICommand, SSHHandler, Playground | ❌ (Not yet documented – contract below) |
| Terminal/CommandResult.cs  | Standardized result object for commands (output string, error, session changes, etc.)                    | (none)          | ICommand, CommandHandler, TerminalStateManager, Playground             | ❌ (Not yet documented – contract below) |
| Terminal/TerminalEngineAPI.cs | Public interface exposing the logic engine to front-end/Playground; entry points for all interaction | (none)          | Unity front-end, Playground/CLI, tests                                 | ❌ (Not yet documented – contract below) |





## CommandHandler.cs — Contract

**Purpose:**  
Central dispatcher for all user commands.  
Receives parsed command name and arguments, plus the current session state/context.  
Finds and executes the appropriate ICommand implementation.  
Never directly implements command logic.

**Responsibilities:**
- Maintains a registry/dictionary of all available commands (`ls`, `cd`, `cat`, etc.), mapping command names/aliases to ICommand implementations.
- When invoked, checks if the command exists and is enabled (including debug flag for debug-only commands).
- Passes session state/context and argument array to the command’s `Execute()` method.
- Returns the command’s result (including output, error, or session state changes) to TerminalStateManager or whoever called it.
- Handles unrecognized commands with a canonical error message (`bash: [cmd]: command not found`).

**NOT Responsible For:**
- Parsing input (handled by CommandParser).
- Maintaining session state (TerminalStateManager does this).
- Printing or outputting directly (returns results to caller).
- Error catching inside command implementations (each command must handle its own errors and report as part of result).
- Auto-discovering commands (registration is explicit).

**Uses:**  
- `ICommand` interface (implemented by all command modules)
- Receives parsed command name/args from CommandParser
- Receives session state from TerminalStateManager

**Used By:**  
- TerminalStateManager (calls to execute commands)
- Playground/CLI test harness

**Completion:** 🚧 Not yet implemented — intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Command System
- Q&A § CommandHandler Modularity, Error Handling, Registration

---

## ICommand (Interface) — Contract

**Purpose:**  
Defines the contract for all command modules (including core, debug, and future commands).

**Responsibilities:**
- Exposes an `Execute(SessionState state, string[] args)` method (or similar).
- Handles all logic for the command, including argument validation, session checks, and error handling.
- Returns a standardized CommandResult (output string, error, state change flag, etc.).

**Completion:** 🚧 Not yet implemented — intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Command System
- Q&A § Command Modularity/Debug

## TerminalStateManager.cs — Contract

**Purpose:**  
Orchestrates all session state, transitions (boot → username assign → shell → SSH), prompt construction, and stack management.  
Owns the current session object (user, device, cwd, SSH stack, debug flag, etc).  
Acts as “the brain” that calls CommandHandler and applies results.

**Responsibilities:**  
- Owns and updates all session state: username, current device, working directory, SSH stack, debug flag, command history.
- Manages state transitions per REFERENCE.md state diagram (including stack overflow/underflow logic for SSH).
- Builds and returns the dynamic prompt string based on session state (username, hostname, cwd) after any state transition or command.
- Applies changes/results from CommandHandler (filesystem, device, cwd changes, etc).
- Suppresses prompt at boot/username/SSH login.
- Returns all output for display; does not directly print or render.

**NOT Responsible For:**  
- Parsing/handling user commands (calls CommandHandler for that).
- Implementing command logic.
- Storing or parsing JSON config (uses managers/loaders).
- Handling direct input/output (only returns output for display layer).

**Uses:**  
- CommandHandler (to run commands)
- FilesystemManager, LoginHandler, SSHHandler, NetworkManager (to enact state changes)
- Session state object

**Used By:**  
- Playground/CLI harness
- (Future) Unity/Front-end consumer

**Completion:** ✅ Implemented

**Cross-Reference:**  
- REFERENCE.md § State Machine, Session State, Prompt Construction
- Q&A § State transitions, stack handling, prompt rules

---
## DebugManager.cs — Contract

**Purpose:**  
Provides all debug/dev-only features, toggles, and info for the logic engine.  
Controls whether debug mode is active, exposes debug-only commands, and manages debug output sections.  
Keeps all debug logic isolated from core session/command logic.

**Responsibilities:**  
- Maintains the global debug flag (on/off).
- Handles the `debug` command to toggle debug mode (can only be run in dev/test builds).
- Registers and manages debug-only commands (e.g., `show filesystems`, `list devices`, `teleport`, `clear debug`), ensuring they are only enabled when debug mode is active.
- Aggregates and returns all debug output sections (e.g., JSON validation errors, session info, internal state) as separate output blocks for the front-end or test harness.
- Ensures all debug output is visually distinct (e.g., bright yellow, [DEBUG] tag in returned strings).
- Allows commands and managers to log debug messages or errors (internal API), but does not process or render them—simply collects and exposes for output.
- Provides methods for other managers/components to check whether debug mode is currently active.

**NOT Responsible For:**  
- Storing session state (TerminalStateManager does this).
- Running or parsing user commands (CommandHandler does this; DebugManager only exposes debug commands for registration).
- Outputting or rendering text (just returns debug info to caller).
- Validating or mutating normal user-facing state.

**Uses:**  
- None directly (holds its own debug command registry and debug state flag).
- May be called by CommandHandler, TerminalStateManager, FilesystemManager, JsonLoader, etc. to check if debug is active, or to log debug info.

**Used By:**  
- CommandHandler (to register/enable debug-only commands)
- TerminalStateManager (to include debug output sections)
- All core managers (to log internal debug messages or errors)
- Playground/CLI/test harness (to display debug output)
- (Future) Unity or front-end layer for dev mode UI

**Completion:** 🚧 Not yet implemented — intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Debug Mode  
- Q&A § Debug/dev commands, error output, debug flag handling

---

**Special Notes:**
- Debug mode **must never be enabled or even visible in release builds**—compile-time flag or build target check required.
- Debug-only commands are registered in CommandHandler, but visibility/executability is controlled by DebugManager.
- All debug info/errors must be segregated from normal terminal output (never mixed in the same buffer/stream).

---

## SessionState.cs — Contract

**Purpose:**  
Stores all mutable session data: username, password (flavor), current device, current working directory, command history, SSH stack (list of session contexts), debug flag, etc.

**Responsibilities:**  
- Provides structured access to all user/session state variables.
- Passed to commands, managers, handlers as needed.
- Resettable (new instance on session restart/reset).

**NOT Responsible For:**  
- Business logic, command parsing, state transitions.
- Exposing public methods (pure data object).

**Used By:**  
- TerminalStateManager, CommandHandler, SSHHandler, all ICommand classes, Playground/CLI.

**Completion:** ❌ Not yet documented/implemented—intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Session State, Prompt Construction, SSH Stack

---

## CommandResult.cs — Contract

**Purpose:**  
Standard return type for all commands.  
Encapsulates command output, error messages, paging flags, session state changes, or other results.

**Responsibilities:**  
- Standardizes the way commands return output/error for display/logging.
- Encapsulates all data needed by the caller (including, if needed, a new SessionState).

**NOT Responsible For:**  
- Business logic or state mutation; just a container for results.

**Used By:**  
- ICommand classes, CommandHandler, TerminalStateManager, Playground/CLI.

**Completion:** ❌ Not yet documented/implemented—intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Command System, Error Handling, Output

---

## TerminalEngineAPI.cs — Contract

**Purpose:**  
Defines the public interface for the env0.terminal logic engine, for front-end (Unity), Playground, or any CLI/test harness.

**Responsibilities:**  
- Exposes all core functionality as public methods/events (e.g., `SendInput(string command)`, `GetPrompt()`, `GetOutput()`, `GetSessionState()`, etc.).
- Provides a stable entry point for initialization, session reset, and integration.
- Hides internal implementation details and private state from the front-end.

**NOT Responsible For:**  
- Implementing core logic (delegates to underlying managers/handlers).
- UI, audio, rendering, or direct user I/O.

**Used By:**  
- Unity front-end, Playground, test suites.

**Completion:** ❌ Not yet documented/implemented—intent and contract documented.

**Cross-Reference:**  
- REFERENCE.md § Integration, Session State, Input/Output, Playground

---
