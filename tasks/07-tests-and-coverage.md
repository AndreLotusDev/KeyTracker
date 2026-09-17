# 07 - Tests & Code Coverage

Test coverage for what was built in tasks 02-04 (the business logic, not the UI).

## Scope

- Unit tests: `KeyboardLayout`/`Key`, in-memory tracking (task 02), migrations (task 03), statistics (task 04)
- Collect coverage with `coverlet` (`dotnet test --collect:"XPlat Code Coverage"`)
- Add the coverage step to `scripts/release.ps1`, before publish
- No arbitrary % gate — cover the real paths (statistics calculation, idempotent migration, key counting), don't write tests just to write tests

## Acceptance criteria

- `dotnet test` runs everything and produces a coverage report
- Statistics, Migrations, and Tracking each have a test covering the happy path and at least one edge case
