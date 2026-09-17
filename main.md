# KeyTracker

## Rules for the agent

- Always keep this file and everything in `/tasks` written in English.
- Minimalist always. No overengineering, no speculative abstraction, no unrequested feature.
- Straight to the point. No flowery text, no redundant summaries, no "ai slop".
- Tasks live in `/tasks`, one per file, numbered in execution order.
- When a task's acceptance criteria are met, **stop and check with the user** before moving to the next one.
- All new code needs tests with coverage (see task 07).
- This document is living: update it as the project evolves, but keep it minimal.

## Overview

Windows app that tracks typed keys, shows a heatmap and a usage dashboard.

## Requirements

- Custom icon
- Starts with Windows
- Generates installer and uninstaller
  - Each iteration generates a new installer
  - Versioned under `/dist`
- Local LiteDB database
  - Migrations for updates between versions
  - Each migration idempotent or transactional when possible
  - Backup the `.db` before a destructive migration
- Structured, readable logs for debugging bugs
- Tracks typed keys on Windows (low-level hook, must not lag the system)
- Heatmap of typed keys
- Support for multiple keyboard layouts
- Dashboard: totals for day, week, month, year + daily average
- Export dashboard as XLSX

## Project structure

```
KeyTracker/
│
├── src/
│   ├── KeyTracker.App/
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   └── App.xaml
│   │
│   ├── KeyTracker.Core/
│   │   ├── Tracking/
│   │   ├── Statistics/
│   │   ├── Keyboards/
│   │   └── Models/
│   │
│   ├── KeyTracker.Infrastructure/
│   │   ├── Database/
│   │   ├── Logging/
│   │   ├── Windows/
│   │   └── Export/
│   │
│   └── KeyTracker.Migrations/
│       ├── Migration001.cs
│       ├── Migration002.cs
│       └── Migration003.cs
│
├── installer/
│   └── setup.iss
│
├── scripts/
│   └── release.ps1
│
└── dist/
    ├── 1.0.0/
    │   └── KeyTracker-Setup-1.0.0.exe
    └── 1.1.0/
        └── KeyTracker-Setup-1.1.0.exe
```

## Database structure

```json
{
  "date": "2026-09-17",
  "layout": "ABNT2",
  "keys": {
    "A": 1543,
    "S": 1382,
    "D": 1102,
    "SPACE": 4821,
    "ENTER": 723
  },
  "total": 58392
}
```

## Dashboard V1

```
┌─────────────────────────────────────────────┐
│ KeyTracker                         Today ▼  │
├─────────────────────────────────────────────┤
│                                             │
│  58.392          51.203          1.24 M     │
│  Today           Daily average   This month │
│                                             │
├─────────────────────────────────────────────┤
│                                             │
│             Keyboard Heatmap                │
│                                             │
│      [Q][W][E][R][T][Y][U][I][O][P]         │
│       [A][S][D][F][G][H][J][K][L]           │
│        [Z][X][C][V][B][N][M]                │
│                                             │
├─────────────────────────────────────────────┤
│ Activity                                    │
│                                             │
│  60k ┤       ╭─╮                            │
│  40k ┤ ╭─╮ ╭─╯ ╰─╮                          │
│  20k ┤─╯ ╰─╯     ╰──                        │
│      └────────────────────────────           │
│        M  T  W  T  F  S  S                  │
│                                             │
│                                [Export XLSX]│
└─────────────────────────────────────────────┘
```

## Logs

```
%LocalAppData%/
└── KeyTracker/
    ├── data/
    │   └── keytracker.db
    │
    └── logs/
        ├── log-20260915.txt
        ├── log-20260916.txt
        └── log-20260917.txt
```

Event types:

```
ApplicationStarted
DatabaseOpened
MigrationStarted
MigrationCompleted
KeyboardHookRegistered
KeyboardHookFailed
ExportStarted
ExportCompleted
UnhandledException
```

## Release

Automated via PowerShell: `./scripts/release.ps1 1.3.0`

```
dotnet test
     ↓
dotnet publish
     ↓
generate executable
     ↓
compile Inno Setup
     ↓
dist/1.3.0/
    ├── KeyTracker-1.3.0.exe
    └── KeyTracker-Setup-1.3.0.exe
```

## Stack

C# + .NET + WPF + LiteDB + Serilog + ClosedXML + Inno Setup.

## Keyboard layouts

Model this from the start — don't hardcode key-by-key in the heatmap.

```
KeyboardLayout
 ├── ABNT2
 ├── ANSI
 ├── ISO
 └── Custom

Key
 ├── ScanCode
 ├── VirtualKey
 ├── Label
 ├── X
 ├── Y
 ├── Width
 └── Height
```

The same statistics render on any layout without rewriting the tracker.

## Tasks

See [`/tasks`](tasks/). Execute in order (01 → 07).
