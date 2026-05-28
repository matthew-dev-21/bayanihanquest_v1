# Bayanihan Quest

**Bayanihan Quest** is a Windows Forms (C#) top-down barangay-themed cleanup and community-building game. Play as a volunteer, collect trash, complete quests, earn rewards, and invest in community improvements through projects, NPC requests, and mini-games.

> School Project — **ITEC 103**

---

## Team

- Matthew Manalang
- Juan Marco Aguilar
- Dave Lorenz Ignacio
- Prince Russel Araneta
- Edrian Manalo Guno

---

## Quick Overview

### Core Loop

1. **Explore the map** and move around the barangay.
2. **Collect trash / ingredients** within pickup range.
3. **Complete quests** (e.g., *Clean Sweep*, *Road Cleanup*) to earn **money** and **reputation**.
4. **Use the store** to buy items that improve performance.
5. **Progress days** and complete daily objectives.
6. **Invest in community** via **Projects**, handle **NPC requests**, and try **Mini-Games**.

### What Makes It “Bayanihan”

The game is designed around helping the community:
- Cleaning public areas
- Supporting barangay improvements
- Participating in community events

---

## Gameplay Features

### Quests

- **Clean Sweep (Map 1)**: Pick up trash around the barangay and report back to the Barangay Captain.
- **Road Cleanup (Map 2)**: Collect trash along the road section in Map 2.

Quests track progress and rewards (money + reputation) through the `Quest` system.

### Player Progression

- **Money**: earned from quests and activities; used in the store.
- **Reputation**: increases your standing (rank title).
- **Stamina**: decreases when doing actions; can be restored.
- **Trash Capacity**: controls how much you can hold.

### Store

A lightweight store modal lets you buy upgrades:
- **Gloves**: faster cleaning
- **Energy Drink**: stamina recovery
- **Trash Bag**: increased trash capacity

### Time / Day System

The game advances an **in-game day counter** based on frame timing. Daily missions and streaks encourage consistent play.

### Community Systems

From within the game:
- **P: Projects** — invest in community upgrades
- **N: NPC** — open NPC requests (daily/rotating)
- **M: Mini-Games** — access mini-games hub

### Events

The game supports **community events** with a goal, progress counter, and end day.

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

## System Design (High-Level)

This project is implemented as a classic WinForms game loop with a timer-driven update cycle.

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

- **LoadingForm**: startup/loading screen, then launches the main menu.
- **MainMenuForm**: start game, load game, feedback, and exit.
- **GameForm**: main gameplay (movement, quests, HUD, interactions, maps).
- **StoreDialog**: store purchases (modal dialog).

#### Models (Game State)

- `Player`: stamina, money, reputation, trash capacity, purchased items.
- `Quest`: progress-based quest object with reward configuration.
- `GameProgressData`: full snapshot of persistent progress.

#### Services (Utilities)

- `AssetLoader`: searches for image assets in startup + assets folders.
- `MapLayoutFactory`: returns walls/spawn areas/interaction zones per map.
- `GameProgressStorage`: saves and loads progress as XML.

---

## Save Data

The game stores your save file as XML here:

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

## How to Run (SharpDevelop)

This is the recommended setup if you will run the project on a laptop using **SharpDevelop**.

### Prerequisites

- Windows
- **SharpDevelop** installed
- **.NET Framework 4.7.2 Developer Pack / Targeting Pack** installed
  - If you only have the runtime, SharpDevelop may open the project but fail to compile.

### Step-by-step (for classmates)

1. **Download the whole project folder**
   - If you got it from GitHub: click **Code → Download ZIP**
   - Extract the ZIP so you have the full folder (don’t open files directly inside the ZIP).

2. **Open the solution in SharpDevelop**
   - Open SharpDevelop
   - Go to **File → Open → Solution**
   - Select `bayanihanquest_v1.sln`

3. **Build the project**
   - Go to **Build → Build Solution**
   - If you see errors about missing framework/version, install the **.NET Framework 4.7.2 Developer Pack**.

4. **Run the game**
   - Set the startup project (if prompted) to `bayanihanquest_v1`
   - Go to **Debug → Start** (or press **F5**)

### Notes

- If images/backgrounds are missing at runtime, ensure the `assets/` folder is present in the extracted project and that you are running from the built output (the game searches for assets relative to the startup path and parent folders).
- Save files are written to `%LOCALAPPDATA%\\Windows_form_game_V1.0\\savegame.xml`.

---

## How to Run (Visual Studio)

### Prerequisites

- Windows
- Visual Studio 2019/2022
- Workload: **.NET desktop development**

### Steps

1. Open the solution: `bayanihanquest_v1.sln`
2. Set the project as Startup Project (if needed): `bayanihanquest_v1`
3. Press **F5** to run.

---

## Troubleshooting

- **Black/blank sprites or missing background**: make sure the image assets exist under `assets/` (the game searches startup folder and parent folders).
- **Save/load not working**: confirm the save file exists at `%LOCALAPPDATA%\\Windows_form_game_V1.0\\savegame.xml`.
- **Different resolutions**: the main menu is maximized and uses dynamic layout; gameplay UI depends on the map panel size.

---

## Notes for Submission

- This repository is a school project for **ITEC 103**.
- If you need a printable project overview, you can reuse the sections above as your documentation outline.
