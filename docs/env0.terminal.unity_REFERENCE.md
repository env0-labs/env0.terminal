# 🧼 RESET.md — Project Baseline (C# Logic Engine, Milestone 1)

This document is the canonical source of truth for the `env0.terminal.unity` C# logic engine as of this reset.  
If a rule or decision isn’t documented here, **assume it is out of scope**—do not implement or assume anything not explicit in this file.  
When in doubt, refer to the full Q&A transcript or request clarification before proceeding.

---

## 🛠 How to Use This File

- **Before coding, changing architecture, or asking for help, check here first.**
- **Never make assumptions about file contents**—if you do not hold the exact schema or code in memory, request it.
- This file is updated whenever major design, logic, or system-level decisions change.

---

## 📓 Project Roles & Boundaries

- **env0.terminal.unity** is a *pure C#* logic engine.
- All terminal, command, filesystem, and network logic is handled here.
- No visual/UI, audio, or front-end responsibilities—these are for Unity or any other consumer via DLL.
- **Milestone 1** goal: *Fully functional, Linux-style virtual terminal with local login, SSH, strict navigation, JSON-driven filesystems, and a clean CLI command system.*

---

## 🖥 Terminal Rendering and Input

- **Terminal buffer**: Text-only, managed in C# (not UI). Output is ephemeral (no scrollback), just like old school terminals.
- **Input**: Insert-only, single-line. ASCII-only for all commands, usernames, passwords, and file content. Non-ASCII input is blocked.
- **Prompt**: Always appended as the last line of output, except during BOOT and LOGIN (local or SSH), where it is suppressed.
- **Command History**:
  - Up to 20 entries, session-only, cleared on restart.
  - Consecutive duplicates are not stored.
  - Failed/mistyped commands are stored.
- **Case Sensitivity**:
  - **All commands, file, and directory lookups are case-insensitive.** (e.g., `cd /Home` == `cd /home`)
  - All JSON keys for files/dirs are normalized to lowercase at load.
- **Clear command**: Wipes only visible terminal content (not history/buffer).
- **Input limit**: 256 characters per command; dangerous/special chars (`;`, `&&`, `|`, etc.) are stripped.
- **Terminal height**: Defaults to 24 rows (classic). Can be set by front-end (e.g., Unity) at initialization; if not set, remains 24. Used for paging in `read` and similar commands.

---

## 🗂 State Machine (TerminalStateManager)

- **States**:
  - BOOT → LOGIN → SHELL
  - From SHELL: may enter SSH (pushes to SSH_STACK)
  - DEBUG is a flag, not a state.
- **SSH Stack**: Max depth of 10. Attempting to SSH deeper:
  - Returns error:  
    `Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.`
  - Forces return to local SBC prompt.
- **Transitions**: Only valid transitions permitted (see full state diagram).

---

## 🔐 Login and SSH Handling

- **Local Login**:
  - On first boot, user creates username/password (ASCII only).
  - Password input is hidden (no echo, no asterisks).
  - Password is stored in plain text for session (not persistent).
  - **Warning:** "WARNING: Do not use a real password. This system is not secure."
  - If password is empty, show:  
    `Yikes... no password? Living dangerously.`
  - Only one user per session/device.
- **SSH**:
  - Supports IP and hostname.
  - `ssh 10.10.10.1` or `ssh workstation.node.zero` → prompt for username & password.
  - `ssh user@host` → prompt for password only.
  - Unlimited login attempts, with randomized delay (2–5 seconds) per attempt (includes password prompt).
  - Failed attempts log `Login failed` and delay before new input.
  - **SSH Hopping**: Supported (can SSH from device to device, up to 10 deep).
  - If SSH stack limit is exceeded, see above.
  - `exit` returns to previous device in stack, or shows:
    `You are already at the local terminal.` at root.
  - **MOTD** is shown on every successful SSH login.
  - Only one user per device (no multi-user, no root/admin distinction).
  - Password input is always hidden.
  - **No Ctrl+C** or manual interrupt.

---

## 💬 Command System

- **Supported Commands**:
  - `ls` / `dir` — List directory contents (dirs first, then files, alpha).
  - `cd` — Change directory (absolute or relative).
  - `cat` — Display file contents (raw text, including .bin, up to 1000 lines). `.sh` returns "file is executable and cannot be read".
  - `read` — Paginated file reader, shows "(empty file)" if file is empty or whitespace only. Can quit with `q`. Page number is shown. Max 1000 lines per file. Page size is terminal height - 1.
  - `echo` — Text output only. No variable substitution, redirection, or escape chars. Multiple spaces, tabs, and special chars are preserved.
  - `clear` — Clears only the current device’s terminal output.
  - `ping` — Simulates 4 packets, random delay (max 100ms), random TTL, random packet loss, per device’s network only.
  - `nmap` — Scans and lists all devices on current network, with open/closed ports. Shows device description, or "No description available".
  - `ssh` — SSH to remote device (see above).
  - `exit` — Close SSH, return to previous session, or shows "You are already at the local terminal." at root.
  - `sudo` — Always returns "Nice try." (Easter egg).
- **Help Command**:
  - `help` shows all commands, grouped by category, paginated like `read`. Aliases (e.g., dir for ls) are shown.
  - Debug commands only shown in help if debug mode is active.
- **Error Handling**:
  - Errors are slightly vague, but clear (e.g., `bash: No such file or directory`). No hints or auto-correction.
  - All errors always leave a blank line below before the prompt.
  - Identical failed command twice: suggest `help`.
  - **Specific errors:**  
    - **SSH stack overflow:**  
      `Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.`
    - **File too large:**  
      `bash: read: File too large (1,000 line limit).`
    - **Command buffer overflow:**  
      `bash: Input exceeds 256 characters. Command ignored.`
    - **Invalid JSON:**  
      `JSON Validation Error: [filename] [details] — using safe defaults.`
    - **Empty password:**  
      `Yikes... no password? Living dangerously.`
    - (Any similar edge case should use a clear, single-line message.)

---

## 📂 Filesystem & Directories

- **JSON-driven, fully loaded at startup** (no dynamic loading).
- Supports max 4 directory levels; filesystems are read-only.
- **Case-insensitive** lookup for all files/dirs; all keys normalized to lower-case at load.
- **File Types**:
  - `.txt`, `.log`, `.bin`: Readable with `cat`/`read` (up to 1000 lines).
    - `.bin` files are flavor files, always nonsense text.
  - `.sh`: Not readable (`cat`/`read` print "file is executable and cannot be read").
- **Safe Mode Filesystem**:
  - Single file (e.g., `congrats.txt`) congratulating the player for breaking in.
- **File Size Limit**:
  - Max 1000 lines per file; larger files fail to open with error.
- **Empty Files/Dirs**:
  - `cat/read` on empty file (or whitespace-only): shows "(empty file)".
  - `ls` on empty dir: shows nothing (just prompt).

---

## 🌐 Network and Devices

- **Devices defined in Devices.json**:
  - `ip` (string)
  - `hostname` (string)
  - `mac` (string)
  - `username` (string)
  - `password` (string)
  - `filesystem` (which FS JSON to load)
  - `motd` (string)
  - `description` (string, optional)
  - `ports` (array)
  - Devices can have multiple interfaces/subnets (e.g., eth0, eth1, wlan0). Each can have their own IP/subnet.
  - Each device has a single user account (no root/admin distinction).
- **Network Commands**:
  - `ifconfig` — Lists all interfaces (e.g., eth0, wlan0), IP, subnet, MAC, per device.
  - `nmap` — Lists all devices on current device's subnet, showing open/closed ports, description.
  - `ping` — Can ping any device on the current network, simulates random delay/loss.
- **Network Topology**:
  - Devices can bridge multiple subnets; overlap/flavor allowed.
  - Network/device list is static at runtime.

---

## ⚡ Debug Mode

- **Only available in dev/debug builds; never shipped in release.**
- **Toggled with `debug` command**; shows “DEBUG MODE ACTIVE” message.
- **Commands** (case-insensitive, not in help by default unless debug is on):
  - `show filesystems` — List all loaded filesystems.
  - `list devices` — Show all network devices (including hidden).
  - `teleport <path>` — Instantly move to any directory (confirmation required).
  - `clear debug` — Clears both main and debug info.
- **JSON validation errors** are shown only in debug mode, in a dedicated section, labeled by JSON type.
- **Debug info** is shown in bright yellow for visibility.
- **Unsafe by design:** debug commands can break normal flow and are for developer use only.

---

## 🧾 JSON Schema Appendix (Summary)

**Filesystem JSON**
```json
{
  "root": {
    "home": {
      "user": {
        "tutorial.txt": {
          "type": "file",
          "content": "Welcome!"
        }
      }
    },
    "etc": {
      "hostname.txt": {
        "type": "file",
        "content": "..."
      }
    }
  }
}



**Devices JSON**
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
      { "name": "eth0", "ip": "10.10.10.1", "subnet": "255.255.255.0", "mac": "00:11:22:33:44:55" },
      { "name": "wlan0", "ip": "10.10.20.1", "subnet": "255.255.255.0", "mac": "00:11:22:33:44:66" }
    ]
  }
]


**BootConfig JSON**
```json
{
  "bootText": [
    "Loading system...",
    "Initializing hardware...",
    "Boot complete."
  ]
}


**UserConfig JSON**
```json
{
  "username": "player",
  "password": "password"
}

🚫 Out of Scope / Not Included (Milestone 1)
No environment variables ($USER, $HOSTNAME)

No file creation, editing, or deletion (read-only FS)

No command chaining or piping

No password complexity enforcement

No multi-user support per device

No non-ASCII input

No Ctrl+C or manual interrupt

No debug mode in shipped builds

No session persistence (always fresh on restart)

No scrollback or output history (ephemeral output only)

No session IDs or exposure to front-end

No runtime device addition/removal

If any rule here is ambiguous or in conflict with another file, this RESET.md takes precedence.

## 🕵️‍♂️ Edge Cases & Validation

These rules eliminate ambiguity and ensure robust, predictable behavior in every possible edge or failure state.  
**If any rule here is violated, the engine must fail fast with a clear error (in debug), or gracefully fallback to a safe state (for players).**

---

### Device Interfaces
- **Every device** in `Devices.json` **must include at least one network interface** (e.g., `eth0`).
- If the `interfaces` array is missing or empty:
    - **In debug mode:** Fail fast with a clear JSON validation error.
    - **In player mode:** Omit the device from all network listings.

---

### MOTD & Ports Field Defaults
- If a device’s `motd` is missing or empty, display the fallback:  
  `Welcome to [hostname]`
- If a device’s `ports` array is missing, treat it as an empty array (no open ports).
    - **In debug mode:** Print a validation warning, but do not break execution.

---

### Filesystem Safe Mode
- A dedicated failsafe filesystem JSON (e.g., `Filesystem_11.json`) **must always be present**.
- If a device’s referenced filesystem is missing or corrupt:
    - Load `Filesystem_11.json` as the safe fallback.
    - This filesystem must contain a single text file (e.g., `/congrats.txt: "You broke in! But there’s nothing here."`).

---

### Debug Mode Statelessness
- Debug mode can only be enabled at startup or toggled via command (if supported).
- Toggling debug mode does **not** reload, wipe, or alter any in-game or session state.
- Debug mode only exposes debug commands and information; no functional side-effects.

---

### Input Handling & Copy-Paste
- **No copy/paste:** User input is strictly key-by-key; pasting is blocked at the engine level.
- All input is ASCII-only, with a maximum of 256 characters per command.
- Special chaining characters (`;`, `&&`, `|`, etc.) are stripped before parsing.
- If non-ASCII or otherwise invalid input is detected, the command is ignored and an error message is shown.

---

### Error Message Verbosity
- **Player mode:** All error messages are terse and slightly vague, following Linux/Unix style.
- **Debug mode:** Validation and internal errors (bad JSON, missing fields, etc.) are fully verbose and shown in a dedicated debug section.
- No internal or verbose errors are ever shown in non-debug builds.

---

### SSH Stack / Failsafe
- If the SSH stack underflows (due to a logic error or bug), forcibly return to the local SBC prompt and print:  
  `Shell state lost; connection reset to local SBC.`
- Always guarantee a way out of any state, even if the SSH stack is corrupted.

---

### Command Buffer Overflow
- If a user enters more than 256 characters (by typing, not pasting), show:  
  `bash: Input exceeds 256 characters. Command ignored.`
- This error is always shown, even in non-debug builds.

## 🕵️‍♂️ Additional Edge Cases & Validation

These rules close off the last possible ambiguities. If anything here is triggered, the engine must fail fast in debug (with clear errors), or gracefully fallback for players.

---

### BootConfig Handling
- If `BootConfig.json` is missing or invalid, skip the boot sequence and print:  
  `Boot sequence unavailable. Skipping to login.`

### Filesystem Reference Errors
- If a device in `Devices.json` references a missing or invalid filesystem, always load the Safe Mode filesystem and, in debug, print a validation error.

### Terminal Height Handling
- If the terminal height is set to zero, negative, or nonsense by the front-end, revert to default (24 rows).

### SSH Stack Overflow Recovery
- If the SSH stack overflows, forcibly discard all nested SSH sessions and reset to the local SBC. Restore all state to pre-SSH values and print:  
  `Stack overflow: Too many nested SSH sessions. Connection reset to local SBC.`

### File/Directory Name Clashes
- If the same name (any case) exists as both a file and a directory at any path level, fail validation in debug mode; in player mode, the directory takes precedence and the file is ignored.

### Device Interface Sanity
- If a device has interfaces with invalid/missing network info, omit those interfaces from network commands. In debug, print a warning.

### Orphaned Devices/No Interfaces
- If a device has no valid interfaces, omit it from all network listings for players. In debug, print a clear JSON validation error.

### UserConfig Recovery
- If `UserConfig.json` is missing or invalid, default to username `player` and password `password`, and print a debug warning.

### JSON Schema Versioning (optional/future-proofing)
- If a loaded JSON file includes a `version` field, use it for validation/migration. If missing, assume version `1.0`.

### Absolute Failures (Unrecoverable)
- If a critical system JSON (like Safe Mode FS) is missing or unreadable at startup, the engine must refuse to start and print a clear error in debug mode.

---

If any of these rules are ambiguous, or if a new edge case is discovered, update this section and document exactly how it should be handled before proceeding.
