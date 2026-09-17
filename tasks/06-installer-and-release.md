# 06 - Installer & Release

Packaging and release automation.

## Scope

- `installer/setup.iss` (Inno Setup): installs the app, creates a shortcut, registers startup with Windows, generates an uninstaller
- `scripts/release.ps1 <version>`: `dotnet test` → `dotnet publish` → compile Inno Setup → produce `dist/<version>/KeyTracker-<version>.exe` and `KeyTracker-Setup-<version>.exe`
- Script fails and stops if `dotnet test` fails (never produce an installer with failing tests)

## Acceptance criteria

- `./scripts/release.ps1 1.0.0` produces the artifacts in `dist/1.0.0/`
- Installer installs, creates a shortcut, registers autostart; uninstaller removes everything cleanly
