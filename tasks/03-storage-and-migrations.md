# 03 - Storage & Migrations

Persist key counters to LiteDB and set up the migrations scheme.

## Scope

- `KeyTracker.Infrastructure/Database`: opens `%LocalAppData%/KeyTracker/data/keytracker.db`, logs `DatabaseOpened`
- One document per day, same shape as `main.md` (`date`, `layout`, `keys`, `total`)
- Periodic flush (e.g. every N seconds or N keys, not every single keystroke) of the in-memory counter (task 02) to disk — batched writes, not per-keystroke I/O
- `KeyTracker.Migrations`: simple runner that applies pending migrations in order, version tracked in the db itself
- Migration idempotent/transactional when possible
- Before a destructive migration, copy the `.db` to `data/backup/keytracker-<version>.db`
- Logs `MigrationStarted` / `MigrationCompleted`

## Acceptance criteria

- Closing and reopening the app preserves the day's counters
- Running the app twice in a row doesn't duplicate or corrupt the day's document
- Simulate a destructive migration and confirm the backup is created before it runs
