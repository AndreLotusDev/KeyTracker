# 03 - Storage & Migrations

Persist key counters to LiteDB and set up the migrations scheme.

## Scope

- [x] `KeyTracker.Infrastructure/Database`: opens `%LocalAppData%/KeyTracker/data/keytracker.db`, logs `DatabaseOpened`
- [x] One document per day, same shape as `main.md` (`date`, `layout`, `keys`, `total`)
- [x] Periodic flush (e.g. every N seconds or N keys, not every single keystroke) of the in-memory counter (task 02) to disk — batched writes, not per-keystroke I/O
- [x] `KeyTracker.Migrations`: simple runner that applies pending migrations in order, version tracked in the db itself
- [x] Migration idempotent/transactional when possible
- [x] Before a destructive migration, copy the `.db` to `data/backup/keytracker-<version>.db`
- [x] Logs `MigrationStarted` / `MigrationCompleted`

## Acceptance criteria

- [x] Closing and reopening the app preserves the day's counters
- [x] Running the app twice in a row doesn't duplicate or corrupt the day's document
- [x] Simulate a destructive migration and confirm the backup is created before it runs
