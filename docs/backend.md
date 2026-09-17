# Backend

Covers `KeyTracker.Core`, `KeyTracker.Infrastructure`, and `KeyTracker.Migrations`.

## Tracking (`Core/Tracking`, `Infrastructure/Windows/KeyboardHook.cs`)

- `KeyboardHook` installs a low-level Windows keyboard hook (`WH_KEYBOARD_LL`) and raises `KeyDown(scanCode, vkCode)` on every `WM_KEYDOWN`/`WM_SYSKEYDOWN`. Runs on the app's message loop; the callback must stay cheap or it lags every keystroke system-wide.
- `KeyPressTracker` (Core) accumulates in-memory counts per scan code between flushes.
- `PeriodicFlushService` (Infrastructure/Database) flushes the tracker to the database every 30s (`App.xaml.cs: FlushInterval`) and once more on app exit (`OnExit`).
- If `SetWindowsHookEx` fails, it logs `KeyboardHookFailed` and the app keeps running without tracking (no crash, no retry).

## Storage (`Infrastructure/Database`)

- `KeyTrackerDatabase` opens the LiteDB connection at `DatabasePaths.DatabaseFilePath` (`%LocalAppData%/KeyTracker/data/keytracker.db`).
- `DailyKeyRecordRepository` upserts one `DailyKeyRecord` per `(date, layout)` — see schema in [main.md](../main.md).
- `LiteDbDailyTotalsSource` / `LiteDbKeyTotalsSource` read aggregated totals for statistics and the heatmap.
- One writer at a time: the periodic flush and the app-exit flush are the only writers; reads happen from the UI thread against the same LiteDB connection.

## Migrations (`KeyTracker.Migrations`)

- `MigrationRunner` runs on every startup, before any other database access (`App.xaml.cs: OnStartup`).
- Applies migrations from `MigrationRegistry.All` in order, tracked via `SchemaInfo`.
- Before a migration marked destructive, it copies the `.db` file into the backup directory (`DatabasePaths.BackupDirectory`).
- Each migration is idempotent or transactional — safe to re-run if the app crashes mid-migration.

## Statistics (`Core/Statistics`)

- `StatisticsCalculator.Summarize(today)` returns `StatisticsSummary` (Today/ThisWeek/ThisMonth/ThisYear/DailyAverage) from `IDailyTotalsSource`.
- `PeriodRange.For(period, today)` maps a `Period` enum value to a `[from, to]` date range, used to scope the heatmap query.
- `ActivityChartBuilder.Build(dailyTotals, granularity)` buckets daily totals into chart points (day/week/month buckets) for the activity chart.

## Export (`Infrastructure/Export/DashboardExporter.cs`)

- `Export(dailyTotals, summary, filePath)` writes an XLSX with two sheets: `Daily` (date, total, sorted ascending by date) and `Summary` (Today/This week/This month/This year/Daily average).
- Logs `ExportStarted` / `ExportCompleted` with the file path.
- Creates the destination directory if missing; overwrites an existing file at `filePath` without prompting (the "already exists" prompt, if any, comes from the OS save dialog, not from this code).

## Windows integration (`Infrastructure/Windows`)

- `TrayIconService`: shows a tray icon with "Open Dashboard" / "Quit"; double-click also opens. Exposes `OpenRequested` / `ExitRequested` events consumed by `App.xaml.cs`.
- `StartupRegistration.Enable(exePath)`: registers the app to start with Windows. Called on every `OnStartup` (idempotent — re-registering with the same path is a no-op in effect).

## Logging (`Infrastructure/Logging/LoggerSetup.cs`)

- Serilog writes structured logs to `%LocalAppData%/KeyTracker/logs/log-YYYYMMDD.txt`.
- Event types are listed in [main.md](../main.md#logs).

## Non-functional requirements

- The keyboard hook callback must not introduce perceptible input lag (no blocking I/O, no allocation-heavy work in `HookCallback`).
- Data loss window on crash is bounded by `FlushInterval` (30s) of unflushed keystrokes.
- Migrations must never silently corrupt the `.db`; destructive migrations always back up first.

## Keep this doc in sync

Update this file whenever tracking, storage, migrations, statistics, export, or Windows-integration code changes behavior — not just signatures.
