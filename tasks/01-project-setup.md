# 01 - Project Setup

Create the base solution structure, no business logic yet.

## Scope

- [x] Solution `.sln` + projects: `KeyTracker.App` (WPF), `KeyTracker.Core`, `KeyTracker.Infrastructure`, `KeyTracker.Migrations`
- [x] Folders per `main.md` (Views/, ViewModels/, Tracking/, Statistics/, Keyboards/, Models/, Database/, Logging/, Windows/, Export/)
- [x] NuGet: LiteDB, Serilog (+ file sink), ClosedXML
- [x] Serilog configured to write to `%LocalAppData%/KeyTracker/logs/log-yyyyMMdd.txt`
- [x] Placeholder icon applied to the `.exe` (can be swapped later, just can't stay the default .NET icon)
- [x] `.gitignore` covering bin/obj/local dist

## Acceptance criteria

- [x] `dotnet build` passes with no new warnings
- [x] App opens an empty WPF window and logs `ApplicationStarted` to the day's file
