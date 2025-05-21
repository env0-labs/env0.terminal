# env0.terminal — Project Overview

---

## What Is This?

**env0.terminal** is a pure C# logic engine for simulating a fully modular, authentic Linux-style terminal.  
It is designed as a black-box DLL for consumption by any front end (Unity, CLI, etc.), powering narrative, puzzle, or adventure games.

- **No UI or rendering is included.**
- All terminal, command, filesystem, and network logic is handled within this C# core.
- World state (filesystems, devices, users, boot sequence) is loaded via JSON.

---

## Project Vision

- **Authenticity:**  
  Emulate the feel and flow of classic Linux/Unix terminals, including realistic error handling and navigation.
- **Modularity:**  
  All world data and rules are JSON-driven for fast iteration and content swapping.
- **Separation of Concerns:**  
  The logic core never touches visuals, audio, or UI—those are always front-end responsibilities.
- **Robustness:**  
  Designed to be “unbreakable” against hostile user input and edge cases.

---

## Repo Orientation

- **REFERENCE.md:**  
  Canonical source of truth for all rules, schemas, system behaviors, and edge-case handling.  
  If you’re implementing, debugging, or expanding the logic engine—**read REFERENCE.md first.**
- **README.md:**  
  The place for the real file/folder structure, developer setup, and quickstart instructions.
- **docs/**:  
  Legacy design docs, full Q&A, task tracking, and project history.

---

## Who Is This For?

- Developers looking to build narrative or puzzle games based on authentic virtual terminals.
- Anyone needing a strict, headless terminal simulation engine for education, testing, or integration with custom front ends.

---

## Where To Start

1. **Read `README.md`** for structure and setup.
2. **Consult `REFERENCE.md`** for all actual implementation rules and schema.
3. **Explore the playground/CLI project** for a quick hands-on test environment.
4. **Open `docs/`** for project history, Q&A, and design evolution (if you’re curious or onboarding a team).

---

## Philosophy

*env0.terminal* is designed for maintainability, strict separation of concerns, and creative extensibility.  
**If you’re unsure whether a feature belongs here, ask:  
“Is it pure terminal logic, or is it presentation/front-end?”  
If in doubt—leave it out.**

---

> **For all technical rules and canonical behaviors, REFERENCE.md is the final word.**
