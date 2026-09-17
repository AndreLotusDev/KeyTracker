# KeyTracker

## Rules for the agent

- Always keep this file, everything in `/tasks`, and everything in `/docs` written in English.
- Minimalist always. No overengineering, no speculative abstraction, no unrequested feature.
- Straight to the point. No flowery text, no redundant summaries, no "ai slop".
- Tasks live in `/tasks`, one per file, numbered in execution order.
- When a task's acceptance criteria are met, **stop and check with the user** before moving to the next one.
- All new code needs tests with coverage (see task 07).
- This document is living: update it as the project evolves, but keep it minimal.
- **Docs in `/docs` are living too.** Whenever a change touches frontend code, backend code, or a feature's behavior, update the matching doc in the same change — [docs/frontend.md](docs/frontend.md), [docs/backend.md](docs/backend.md), or the relevant page under [docs/business/](docs/business/). Stale docs are treated as a bug, since e2e tests are written from them.
- **New tasks follow this format:** a `## Scope` list and an `## Acceptance criteria` list, both as `- [ ]` checkboxes. Check a box (`- [x]`) only when that item is actually done in the code, not when it's planned. Update the [Task status](#task-status) table in the same change.

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

## Documentation

Detailed, living docs live in `/docs`, split by layer:

- [docs/frontend.md](docs/frontend.md) — WPF views, view models, bindings, window lifecycle.
- [docs/backend.md](docs/backend.md) — tracking, storage, migrations, statistics, export, Windows integration, logging.
- [docs/business/](docs/business/) — one file per feature/screen ("page"), describing how it behaves for a user. Current pages: [dashboard.md](docs/business/dashboard.md), [tray-icon.md](docs/business/tray-icon.md), [export-xlsx.md](docs/business/export-xlsx.md).

Keep them updated whenever the corresponding code changes (see rules above). These docs are the source of truth used to write e2e tests — if a doc doesn't match behavior, fix the doc or the code, don't leave the mismatch.

### Template for a business doc page

Every file under `docs/business/` follows this structure:

```markdown
# Page: <Name>

## Purpose
One or two sentences: what this page/feature is for.

## How it works
Short description of the mechanism: what triggers it, what data it reads/writes, what it renders.

## Expected flows
### Flow: <name>
1. Step
2. Step
3. Step

(one subsection per distinct user flow, including edge/cancel paths)

## Expected results
Concrete, checkable outcomes for each flow (what should be true after it runs, including edge cases like empty data).

## Functional requirements
- FR1: ...
- FR2: ...

## Non-functional requirements
- NFR1: ...

## Out of scope / known limits
What this page intentionally does not do.
```

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

Storage/migration details: [docs/backend.md](docs/backend.md).

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

Event types are listed in [docs/backend.md](docs/backend.md#logging-infrastructureloggingloggersetupcs).

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

See [`/tasks`](tasks/). Execute in order (01 → 08).

### Task status

| # | Task | Status |
|---|------|--------|
| 01 | [project-setup](tasks/01-project-setup.md) | Done |
| 02 | [keyboard-tracking](tasks/02-keyboard-tracking.md) | Done |
| 03 | [storage-and-migrations](tasks/03-storage-and-migrations.md) | Done |
| 04 | [statistics-and-export](tasks/04-statistics-and-export.md) | Done |
| 05 | [dashboard-ui](tasks/05-dashboard-ui.md) | Done |
| 06 | [installer-and-release](tasks/06-installer-and-release.md) | Done |
| 07 | [tests-and-coverage](tasks/07-tests-and-coverage.md) | Done |
| 08 | [e2e-tests](tasks/08-e2e-tests.md) | Not started |
