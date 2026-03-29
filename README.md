# LightRP

A simple, easily editable roleplay framework for **S&Box** (Source 2).
Inspired by DarkRP, LightRP aims to be a playable foundation that server
owners can extend and customise without fighting the codebase.

## Features

| Feature | Description |
|---------|-------------|
| **Job System** | Pre-defined jobs (Citizen, Police, Mayor, Medic, …) with salary, colour, slot limits and categories. |
| **Economy** | Per-player wallet with automatic salary payments on a configurable timer. |
| **Player State** | Networked component tracking each player's RP name, job and money. |
| **HUD** | Minimal overlay displaying job, wallet balance and RP name. |
| **Job Menu** | In-game panel for browsing and joining available jobs (press **Tab**). |
| **Chat Commands** | `/job`, `/jobs`, `/money`, `/name`, `/give`, `/help`. |
| **Game Manager** | Central orchestrator handling player connections and defaults. |

## Project Structure

```
Code/
├── Assembly.cs              Global using statements
├── GameManager.cs           Core game-mode orchestrator
├── Chat/
│   └── ChatCommands.cs      Slash-command handler
├── Jobs/
│   ├── JobDefinition.cs     Data class for a single job
│   └── JobManager.cs        Static job registry
├── Player/
│   └── PlayerState.cs       Networked player RP state
└── UI/
    ├── RpHud.razor           Bottom-left HUD overlay
    ├── RpHud.razor.scss      HUD styles
    ├── JobMenu.razor          Job selection panel
    └── JobMenu.razor.scss     Job menu styles
```

## Getting Started

1. Open the project in S&Box.
2. Load `scenes/minimal.scene`.
3. Add the **GameManager** component to a GameObject in the scene.
4. Add the **RpHud** and **JobMenu** panel components to a screen panel.
5. Press **Play** — players will be assigned the *Citizen* job by default.

## Adding or Editing Jobs

Open `Code/Jobs/JobManager.cs` and edit `RegisterDefaultJobs()`:

```csharp
Register( new JobDefinition
{
    Name        = "Chef",
    Description = "Cook food and sell it to hungry citizens.",
    Color       = new Color( 1f, 0.6f, 0f ),
    Salary      = 50,
    MaxSlots    = 2,
    Category    = "Civilian"
} );
```

You can also register jobs at runtime:

```csharp
JobManager.Register( myCustomJob );
```

## Chat Commands

| Command | Description |
|---------|-------------|
| `/job <name>` | Switch to a job |
| `/jobs` | List all available jobs |
| `/money` | Check your wallet balance |
| `/name <name>` | Change your RP name |
| `/give <player> <amount>` | Transfer money to another player |
| `/help` | Show the command list |

## Configuration

Key settings live on the **GameManager** component (inspector-editable):

- **DefaultJobName** — Job assigned on connect (default: `Citizen`)
- **DefaultStartingMoney** — Initial wallet balance (default: `500`)
- **DefaultPaydayInterval** — Seconds between salary payments (default: `120`)

Per-job settings (salary, max slots, vote requirements) are configured
directly in `JobManager.RegisterDefaultJobs()`.

## Extending LightRP

- **New systems** — Create a new `Component` and attach it to the player
  or a scene object. Follow the same patterns used by `PlayerState`.
- **New commands** — Add a case to the switch in `ChatCommands.TryHandle()`.
- **New UI** — Create a `.razor` / `.razor.scss` pair under `Code/UI/`.

## License

This project is open source. See the repository for license details.
