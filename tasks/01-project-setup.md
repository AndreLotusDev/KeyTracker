# 01 - Project Setup

Create the base solution structure, no business logic yet.

## Scope

- Solution `.sln` + projects: `KeyTracker.App` (WPF), `KeyTracker.Core`, `KeyTracker.Infrastructure`, `KeyTracker.Migrations`
- Folders per `main.md` (Views/, ViewModels/, Tracking/, Statistics/, Keyboards/, Models/, Database/, Logging/, Windows/, Export/)
- NuGet: LiteDB, Serilog (+ file sink), ClosedXML
- Serilog configured to write to `%LocalAppData%/KeyTracker/logs/log-yyyyMMdd.txt`
- Placeholder icon applied to the `.exe` (can be swapped later, just can't stay the default .NET icon)
- `.gitignore` covering bin/obj/local dist

## Acceptance criteria

- `dotnet build` passes with no new warnings
- App opens an empty WPF window and logs `ApplicationStarted` to the day's file
