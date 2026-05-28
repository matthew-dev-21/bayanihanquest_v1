<div align="center">

# Bayanihan Quest

Top-down barangay cleanup and community-building game built with **C# WinForms**.

**School Project — ITEC 103**

<p>
  <img alt="Platform" src="https://img.shields.io/badge/Platform-Windows-informational" />
  <img alt="UI" src="https://img.shields.io/badge/UI-WinForms-blue" />
  <img alt="Framework" src="https://img.shields.io/badge/.NET%20Framework-4.7.2-purple" />
  <img alt="Save" src="https://img.shields.io/badge/Save-XML%20Serialization-success" />
</p>

<p>
  <a href="#getting-started-sharpdevelop">Getting Started</a> •
  <a href="#controls">Controls</a> •
  <a href="#gameplay">Gameplay</a> •
  <a href="#system-design">System Design</a> •
  <a href="#save-data">Save Data</a>
</p>

</div>

---

## Table of Contents

- [Team](#team)
- [Getting Started (SharpDevelop)](#getting-started-sharpdevelop)
- [Getting Started (Visual Studio)](#getting-started-visual-studio)
- [Quick Overview](#quick-overview)
- [Gameplay](#gameplay)
- [Controls](#controls)
- [System Design](#system-design)
- [Save Data](#save-data)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Troubleshooting](#troubleshooting)
- [Notes for Submission](#notes-for-submission)

---

## Team

- Matthew Manalang
- Juan Marco Aguilar
- Dave Lorenz Ignacio
- Prince Russel Araneta
- Edrian Manalo Guno

---

## Getting Started (SharpDevelop)

This is the recommended setup if you will run the project on a laptop using **SharpDevelop**.

### Prerequisites

- Windows
- SharpDevelop installed
- **.NET Framework 4.7.2 Developer Pack / Targeting Pack** installed
  - If you only have the runtime, SharpDevelop may open the project but fail to compile.

### Step-by-step (for classmates)

1. Download the whole project folder
   - If you got it from GitHub: **Code → Download ZIP**
   - Extract the ZIP (don’t open files directly inside the ZIP)

2. Open the solution
   - SharpDevelop → **File → Open → Solution**
   - Select `bayanihanquest_v1.sln`

3. Build
   - **Build → Build Solution**
   - If you see missing framework/version errors, install the **.NET Framework 4.7.2 Developer Pack**

4. Run
   - If prompted, set startup project to `bayanihanquest_v1`
   - **Debug → Start** (or `F5`)

### Notes

- If images/backgrounds are missing at runtime, ensure the `assets/` folder exists in the extracted project. The game searches for assets relative to the startup path and parent folders.
- Save files are written to `%LOCALAPPDATA%\\Windows_form_game_V1.0\\savegame.xml`.

---

## Getting Started (Visual Studio)

### Prerequisites

- Windows
- Visual Studio 2019/2022
- Workload: **.NET desktop development**

### Steps

1. Open `bayanihanquest_v1.sln`
2. Set startup project (if needed): `bayanihanquest_v1`
3. Run with `F5`

---

## Quick Overview

### Core Loop

1. Explore the map
2. Collect trash / ingredients within pickup range
3. Complete quests to earn money and reputation
4. Use the store to buy upgrades
5. Progress days and complete daily objectives
6. Invest in community via projects, NPC requests, and mini-games

### What Makes It “Bayanihan”

The game is designed around helping the community through cleanup, improvements, and events.

---

## Gameplay

### Quests

- **Clean Sweep (Map 1)**: Pick up trash around the barangay and report back to the Barangay Captain.
- **Road Cleanup (Map 2)**: Collect trash items along the road section in Map 2.

Quests track progress and rewards (money + reputation) through the `Quest` system.

### Player Progression

- **Money**: earned from quests and activities; used in the store
- **Reputation**: increases your standing (rank title)
- **Stamina**: decreases when doing actions; can be restored
- **Trash Capacity**: controls how much you can hold

### Store

Buy upgrades through a store modal:

- **Gloves**: faster cleaning
- **Energy Drink**: stamina recovery
- **Trash Bag**: increased trash capacity

### Time / Day System

The game advances an in-game day counter based on frame timing. Daily missions and streaks encourage consistent play.

### Community Systems

Accessible through hotkeys inside the game:

- Projects
- NPC requests
- Mini-games

### Events

Supports community events with a goal, progress counter, and end day.

---

## Controls

| Action | Key |
|---|---|
| Move Up | W |
| Move Down | S |
| Move Left | A |
| Move Right | D |
| Interact / Confirm | E |
| Community Projects | P |
| NPC Requests | N |
| Mini-Games Hub | M |
| Exit (close game form) | Esc |

---

## System Design

This project uses a classic WinForms, timer-driven game loop.

### Architecture Map

```mermaid
flowchart TD
  A[Program.cs] --> B[LoadingForm]
  B --> C[MainMenuForm]
  C -->|Start / Load| D[GameForm]

  D --> E[Models]
  D --> F[Services]
  D --> G[Forms (UI Components)]

  E --> E1[Player]
  E --> E2[Quest]
  E --> E3[TrashItem / IngredientItem]
  E --> E4[GameProgressData]

  F --> F1[AssetLoader]
  F --> F2[MapLayoutFactory]
  F --> F3[GameProgressStorage (XML Save/Load)]

  D -->|Save/Load| F3
```

### Key Components

#### Forms (UI)

- **LoadingForm**: startup/loading screen, then launches the main menu
- **MainMenuForm**: start game, load game, feedback, and exit
- **GameForm**: main gameplay (movement, quests, HUD, interactions, maps)
- **StoreDialog**: store purchases (modal dialog)

#### Models (Game State)

- `Player`: stamina, money, reputation, trash capacity, purchased items
- `Quest`: progress-based quest object with reward configuration
- `GameProgressData`: full snapshot of persistent progress

#### Services (Utilities)

- `AssetLoader`: searches for image assets in startup + assets folders
- `MapLayoutFactory`: returns walls/spawn areas/interaction zones per map
- `GameProgressStorage`: saves and loads progress as XML

---

## Save Data

Save file location:

- `%LOCALAPPDATA%\\Windows_form_game_V1.0\\savegame.xml`

This is handled by `GameProgressStorage` using .NET XML serialization.

---

## Project Structure

```
.
├─ bayanihanquest_v1.sln
├─ bayanihanquest_v1/
│  ├─ Program.cs
│  ├─ Forms/
│  ├─ Models/
│  ├─ Services/
│  ├─ Properties/
│  └─ assets/
└─ README.md
```

---

## Tech Stack

- **Language**: C#
- **UI**: Windows Forms
- **Framework**: .NET Framework 4.7.2
- **Persistence**: XML serialization (save file)

---

## Troubleshooting

- **Black/blank sprites or missing background**: confirm `assets/` exists (the game searches startup folder and parent folders).
- **Save/load not working**: confirm the save file exists at `%LOCALAPPDATA%\\Windows_form_game_V1.0\\savegame.xml`.
- **Different resolutions**: the main menu is maximized and uses dynamic layout; gameplay UI depends on the map panel size.

---

## Notes for Submission

- This repository is a school project for **ITEC 103**.
- This README is written as a full game system overview for documentation/submission.
