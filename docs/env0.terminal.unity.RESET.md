# 🧼 RESET.md — Project Baseline (C# Logic Engine)

This document captures the known-good state of env0.core’s **C# terminal logic engine** as of the latest stable reset.  
Use this as the single source of truth for roles, system boundaries, architectural intent, and what’s working or in-scope as of this baseline.

**This file explicitly applies to the pure C# logic layer only. No Unity code, assets, or FX are considered here.**

---

## 🛠 How to Use This File

1. **Before** implementing any feature, changing architecture, or seeking advice, check this file.
2. If a feature or approach isn’t described here, **do not automatically treat it as out-of-scope**—always confirm with the project lead (user) before proceeding or discarding.
3. Any code suggestion must reference the actual file contents—**never rely on summaries or prior versions**. If unsure, **request the file and operate only with what is actually present**.

---

## 🖥 Terminal Rendering and Input Handling

### ✔ Core Behavior

- Terminal rendering is simulated via a simple console buffer for testing, or through any frontend that connects to the C# engine.
- Input is captured and routed through `TerminalManager`.
- Output is buffered and displayed, maintaining scrollback and cursor state (including logical cursor movement for inline editing).

---

## 🔐 Login Flow

- Local login flow is supported: users create a username/password when starting a session.
- SSH login is fully separated from local login and managed as a distinct flow.
- Credentials, device mapping, and authentication logic are all managed by the C# core and loaded from JSON.

---

## 🚀 Boot/Init Sequence

- Boot sequence logic is modular, loaded from JSON (`BootConfig.json`).
- Supports atmospheric boot text and skip functionality.
- Triggers a clear transition from boot → login → shell state.

---

## 💬 CLI Command System

- Modular command system: each command is a separate class.
- Commands route through `CommandParser`.
- Commands currently implemented: `ls`, `cd`, `clear`, `cat`, `read`, `ping`, `nmap`, `ifconfig`.
- All command, error, and system output is handled by the C# logic engine.

---

## 📂 Filesystem & Devices

- Virtual filesystem(s) are fully JSON-driven; structure and contents loaded at runtime.
- Devices and network state are managed via JSON, with each device mapping to its own filesystem.
- All system logic (navigation, file reading, SSH) runs in C# only—no Unity or external dependencies.

---

## 🚫 Out of Scope / Not Included

- No Unity visuals, TextMeshPro, shaders, FX, or audio.
- No Unity asset pipeline, scenes, prefabs, or MonoBehaviours.
- No menu or UI layer beyond what is needed for pure terminal interaction.
- No game logic dependent on Unity scene objects.

---

## 🧊 Summary

- **Baseline:** env0.core C# logic engine is modular, JSON-driven, and testable in isolation.
- **All rendering, audio, and advanced FX are deferred to Unity or other frontends—not handled here.**
- This file is the source of technical truth for what constitutes “done” at any reset point.

---

## 🗂 Supplemental Architecture Files

- See `/docs/milestones.md` for milestone breakdowns.
- Additional architectural references and contracts will be added as the project expands.
- **NEW:** `TerminalEngine.cs` acts as the single public API and state gatekeeper for the entire logic engine. All input, state transitions, prompt rendering, and shell command handling are routed through this class, ensuring the console app and all future front-ends remain cleanly decoupled from internal module logic.


---

## 🛑 Anti-Assumption Policy

**Never make assumptions about the contents or behavior of any code file.**  
If the precise contents are not present and visible in this context, **ask to see the file before providing technical advice, refactoring, or usage suggestions**.

This is essential to avoid:
- AI hallucinations about code structure or APIs
- Introducing subtle bugs through misremembered logic
- Repeating past mistakes or inconsistencies

**If in doubt, request the file and operate only with what is actually present.**
