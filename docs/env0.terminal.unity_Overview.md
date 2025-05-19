# 📌 env0.terminal.unity Project Overview (C# Logic Engine)

This document provides a comprehensive, detailed overview of the env0.terminal.unity core logic engine, including architecture, system design, and all key implementation rules.  
**This is an implementation overview, not a replacement for RESET.md. RESET.md is always the source of truth.**

---

## 🚀 Project Vision and Scope

**env0.terminal.unity** is a modular, JSON-driven terminal simulation engine written in pure C#.  
It is designed to act as a “black box” DLL logic engine, exposing all command and terminal logic for consumption by any front-end (such as Unity).  
The aim is to create an authentic, old-school Linux/Unix-style terminal, supporting:
- Local login (username/password)
- SSH with stack-based SSH hopping
- Strict, read-only, JSON-defined virtual filesystems
- Static network exploration and device access
- Accurate command, error, and output formatting

---

## 📂 Project Structure

**Directory Layout**


/src/env0.terminal (Core Logic)
├── /Terminal
│ ├── TerminalManager.cs // Main input/output handler
│ ├── TerminalStateManager.cs // State management (boot, login, shell, SSH stack)
│ ├── CommandParser.cs // Command parsing and routing
│ └── /Commands
│ ├── CommandHandler.cs // Central command execution
│ ├── LsCommand.cs // List directory contents
│ ├── CdCommand.cs // Change directory
│ ├── CatCommand.cs // Display file contents
│ ├── ReadCommand.cs // Paginated file reader
│ ├── EchoCommand.cs // Text output
│ ├── PingCommand.cs // Network ping
│ ├── NmapCommand.cs // Network scan
│ ├── SshCommand.cs // SSH to remote device
│ ├── ClearCommand.cs // Clear screen
│ └── SudoCommand.cs // Easter egg ("Nice try.")
├── /Boot
│ ├── BootSequenceHandler.cs // Boot sequence logic
│ └── BootConfig.json // Boot text
├── /Login
│ ├── LoginHandler.cs // Local/SSH login logic
│ ├── UserManager.cs // User/session management
│ └── UserConfig.json // Local user settings
├── /Filesystem
│ ├── FileSystemManager.cs // Filesystem logic
│ ├── FileSystemLoader.cs // Loads all FS JSON at startup
│ ├── FileSystemEntry.cs // Directory/file model
│ └── /Files
│ ├── Filesystem_1.json // Primary FS
│ └── Filesystem_10.json // "Safe mode" FS
└── /Network
├── NetworkManager.cs // Device connection logic
├── Devices.cs // Manages all devices
├── Device.cs // Device structure
└── Devices.json // Network/device definitions



---

## 🔧 System Architecture & Rules

### Terminal Handling

- **Input:** Insert-only, single-line, ASCII-only. Non-ASCII is blocked.
- **Prompt:** Full absolute path (e.g., `ewan@sbc:/home/docs$`) is always appended as last line of output, except in BOOT/LOGIN state.
- **Command history:**  
    - Session-only (cleared on restart)
    - Max 20 entries, no consecutive duplicates, failed commands are stored
    - Up/Down for history, Left/Right for inline cursor movement
- **No scrollback:** Output is ephemeral. `clear` wipes visible output only.
- **Terminal height:** Default is 24 rows (classic). Configurable by front-end at initialization for correct paging.

---

### State Machine

- **States:** BOOT → LOGIN → SHELL; from SHELL, user may enter SSH (stacked, up to 10 deep)
- **SSH stack:** Max depth 10. Exceeding limit resets to local SBC and prints:
  `Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.`
- **DEBUG is a flag, not a state.**

---

### Boot Sequence

- **Loaded from BootConfig.json**
- User may skip boot with Tab (prompt appears at end)
- After boot, transitions to LOGIN

---

### Login and SSH

#### Local Login

- User creates a username and password (ASCII-only, insert-only, single line)
- Password is hidden as typed (no asterisks or echo)
- Empty password is allowed, but prints:
  `Yikes... no password? Living dangerously.`
- **Security warning:**  
  `"WARNING: Do not use a real password. This system is not secure."`
- Only one user per session/device

#### SSH

- Supports `ssh <ip>` / `ssh <hostname>` (prompts for username and password)
- Supports `ssh user@host` (prompts for password only)
- Unlimited login attempts, randomized delay (2–5 sec) for realism
- Failed attempts print `Login failed` after delay
- **SSH Hopping** supported (can SSH from device to device, up to 10 deep)
- Exceeding SSH stack limit resets to local prompt with error
- `exit` returns to previous device or prints  
  `You are already at the local terminal.` if at SBC
- MOTD is shown on every successful SSH login (from device JSON)
- Only one user per device (no multi-user, root, or admin distinction)
- No Ctrl+C or interrupt

---

### Filesystem Management

- **Fully JSON-driven, loaded at startup (no dynamic loading)**
- **Case-insensitive lookup:**  
  All keys/paths normalized to lowercase on load. No duplicates allowed, regardless of case.
- **Read-only:** No file creation, editing, deletion in Milestone 1
- **4 directory levels max**
- **Supported file types:**  
    - `.txt`, `.log`, `.bin` are readable (`cat`, `read`); max 1,000 lines.  
      `.bin` files show nonsense flavor text.
    - `.sh` cannot be read:  
      `"file is executable and cannot be read"`
    - Unsupported extensions are silently treated as plain text
- **Safe Mode FS:** If device FS is missing/invalid, loads a single-file “congrats.txt” filesystem
- **Empty file:** `(empty file)` if content is empty/whitespace only
- **ls on empty dir:** Just prompt, no text
- **File size cap:** >1000 lines fails with:  
  `bash: read: File too large (1,000 line limit).`

---

### Network and Devices

- **Devices in Devices.json:**
    - `ip`, `hostname`, `mac`, `username`, `password`, `filesystem`, `motd`, `description` (optional), `ports` (array), `interfaces` (array of interface objects with name/ip/subnet/mac)
- **Multiple interfaces per device are supported (eth0, wlan0, etc)**
- **Network is static at runtime**
- **Network commands:**
    - `ifconfig`: Lists all interfaces per device, IP, subnet, MAC
    - `nmap`: Lists all devices on current device’s subnet, all open/closed ports, description or “No description available”
    - `ping`: Always 4 packets, random delay/loss/TTL, no flags

---

### Command System

- **Supported commands:**
    - `ls` / `dir`: List directory contents (dirs first, then files, alpha)
    - `cd`: Change directory (absolute/relative, case-insensitive, supports `..`, `.`, `~`)
    - `cat`: Print file (raw, up to 1,000 lines, all types except `.sh`)
    - `read`: Paginated file reader; shows page numbers, `(empty file)` for empty/whitespace-only, `q` to quit
    - `echo`: Print literal text (no variables, no redirection)
    - `clear`: Clear visible output only
    - `ping`: Simulate 4 packets, random delay/loss
    - `nmap`: Scan network (current subnet), open/closed ports
    - `ssh`: Connect to remote device (as above)
    - `exit`: Close SSH or print “You are already at the local terminal.”
    - `sudo`: Always returns “Nice try.”
- **Aliases:**
    - `dir` is a direct alias for `ls` (listed in help)
- **Help:** Lists all available commands (grouped, with usage/examples), paginated. Debug commands shown only if debug mode is active.
- **Error handling:**  
    - Linux-like, always leaves a blank line before prompt
    - Identical failed command twice suggests `help`
    - **Specific error messages:**  
        - SSH stack overflow, file too large, buffer overflow, invalid JSON, empty password, etc.

---

### Debug Mode

- **Debug mode is only available in dev/debug builds**
- **Toggled via `debug` command** (prints DEBUG MODE ACTIVE)
- **Debug commands:**  
    - `show filesystems`  
    - `list devices`  
    - `teleport <path>` (confirmation required)  
    - `clear debug`
- Debug commands case-insensitive; listed in help only if debug is on
- Debug info in bright yellow
- JSON validation errors shown only in debug, in a labeled section
- Debug mode is unsafe by design—intended for dev only

---

### JSON Configuration Standards

#### BootConfig.json
```json
{
  "bootText": [
    "Loading system...",
    "Initializing hardware...",
    "Boot complete."
  ]
}


#### UserConfig.json
```json


{
  "username": "player",
  "password": "password"
}
#### Filesystem_1.json (Example)
```json

{
  "root": {
    "home": {
      "user": {
        "welcome.txt": {
          "type": "file",
          "content": "Welcome to your basic UNIX machine!"
        }
      }
    },
    "etc": {
      "hostname.txt": {
        "type": "file",
        "content": "Basic UNIX Machine"
      }
    }
  }
}
#### Devices.json (Example)
```json
[
  {
    "ip": "10.10.10.1",
    "hostname": "workstation.node.zero",
    "mac": "00:11:22:33:44:55",
    "username": "admin",
    "password": "password",
    "filesystem": "Filesystem_1.json",
    "motd": "Welcome to workstation.node.zero",
    "description": "Primary user workstation",
    "ports": ["22", "80"],
    "interfaces": [
      { "name": "eth0", "ip": "10.10.10.1", "subnet": "255.255.255.0", "mac": "00:11:22:33:44:55" }
    ]
  }
]


#### 🚫 Not Included in Milestone 1
No file creation, editing, or deletion

No environment variables ($USER, $HOSTNAME)

No command chaining/piping, autocomplete, or tab-completion

No password complexity enforcement

No multi-user support

No non-ASCII input anywhere

No Ctrl+C or interrupt

No debug mode in shipped builds

No session persistence (always fresh on restart)

No scrollback or output history

No session IDs or front-end state

No runtime device addition/removal

If any aspect of this overview conflicts with RESET.md, RESET.md always takes precedence.
When in doubt: request clarification, do not assume.