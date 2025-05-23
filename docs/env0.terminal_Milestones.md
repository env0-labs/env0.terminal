# MILESTONES.md

This document defines major project milestones for `env0.terminal`.  
Milestones 1a and 1b are strictly defined.  
Milestones 2+ are directional and may be revised as project needs evolve.

---

## Milestone 1a — Terminal Core, Single Filesystem (IN MEMORY)

- **Objective:**  
  Build a fully working core terminal experience with hardcoded in-memory filesystem, basic navigation, and command execution.
- **Scope:**  
  - Terminal command loop accepts raw user input.
  - Commands implemented: `ls`, `cd`, `cat` (strict, Linux-like error handling).
  - Hardcoded, in-memory filesystem (`FileEntry` or equivalent).
  - No JSON loading, device abstraction, or network context.
  - Command handler/dispatcher in place.
  - Filesystem operations function as intended; errors on invalid paths/commands.
  - Minimal but clear test coverage for all commands and core FS logic.
  - Manual gameplay tests prove system loop.
- **Exclusions:**  
  - No device/network abstraction.
  - No SSH/login.
  - No JSON file loading.
  - No advanced features (autocomplete, history, FX, etc).

---

## Milestone 1b — JSON Loader, Devices, and Network Context

- **Objective:**  
  Modularize filesystem loading, add device/network context, and enable SSH/login simulation.
- **Scope:**  
  - Filesystem loads from external JSON files (not hardcoded).
  - Device abstraction: multiple filesystems/devices, each with its own config.
  - SSH/login flow: context switch updates prompt, filesystem, and session state.
  - JSON loader/POCO model robust to malformed or missing data (with fallback or error).
  - Error handling for failed device/SSH operations.
  - Integration test (“Milestone Master Test”) proves full command-to-output pipeline across device/session boundaries.
- **Exclusions:**  
  - No full permissions or advanced security.
  - No multi-user logic.
  - No narrative or AI program.
  - Minimal network realism—focus is on working abstraction, not security or simulation.

---

## Milestone 2 — Conversational Narrative Layer

- **Objective:**  
  Integrate an adventure-style, AI-powered program into the terminal.
- **Scope (directional):**  
  - Launchable in-terminal “AI” with verb+noun parser.
  - Predefined response arrays, branching hints, and narrative fragments.
  - Supports both “System AI” (polite, robotic) and “Cursed AI” (hostile/sarcastic) modes.
  - Player interaction with the AI influences story/progression.

---

## Milestone 3 — Seamless Expandability

- **Objective:**  
  Ensure the system is easy to expand, update, and maintain.
- **Scope (directional):**  
  - All new devices, filesystems, commands, and narrative content can be added via JSON or external config/tools.
  - No hardcoding of future expansion.
  - Documentation and modular architecture enforced.

---

## Milestone 4 — Immersive Presentation & Atmosphere

- **Objective:**  
  Create a visually and sonically authentic “haunted terminal” experience.
- **Scope (directional):**  
  - Retro visuals, terminal FX, ambient/interactive audio.
  - Visual and audio feedback matches narrative tone.
  - Presentation handled at the Unity (or chosen frontend) layer, not in core logic.

---

## Milestone 5+ — Ongoing Evolution

- Additional milestones to be defined based on project needs, creative direction, and user feedback.
- Examples: Advanced security, multiplayer, world/story expansion, editor tooling, etc.

---

**Milestones are living documents—1a and 1b are locked for v1.0, later milestones will adapt as required.**
