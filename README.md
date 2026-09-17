# KeyTracker

Windows app that tracks typed keys, shows a heatmap, and a usage dashboard.

## Stack

C# + .NET + WPF + LiteDB + Serilog + ClosedXML + Inno Setup.

## Features

- Low-level keyboard hook, tracks keys without lagging the system
- Heatmap of typed keys, supports multiple keyboard layouts
- Dashboard: totals for day/week/month/year + daily average
- Export dashboard as XLSX
- Starts with Windows, tray icon
- Local LiteDB database with versioned migrations
- Installer/uninstaller generated per release

## Documentation

- [main.md](main.md) — project overview, requirements, structure, release process, task list
- [docs/frontend.md](docs/frontend.md) — WPF views, view models, bindings
- [docs/backend.md](docs/backend.md) — tracking, storage, migrations, statistics, export, logging
- [docs/business/](docs/business/) — feature behavior per page

## Release

```bash
./scripts/release.ps1 1.3.0
```
