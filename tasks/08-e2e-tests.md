# 08 - E2E Tests

End-to-end tests driving the real WPF app, written from `docs/business/`.

## Scope

- [ ] Drive the packaged/built app (not just view models) through its actual UI: tray icon, dashboard window, heatmap, export
- [ ] One suite of scenarios per page under `docs/business/` (dashboard, tray-icon, export-xlsx), following each page's "Expected flows"
- [ ] Use a disposable/temp LiteDB file per test run, seeded with known key data, so assertions are deterministic
- [ ] Assert against each page's "Expected results", not implementation details
- [ ] Add the e2e run as a separate step in `scripts/release.ps1` (after unit tests, before publish), so a failing e2e blocks the release

## Acceptance criteria

- [ ] Each flow listed in `docs/business/*.md` has a passing e2e test
- [ ] Tests run against a real built app instance, not mocks of the UI layer
- [ ] A doc/behavior mismatch fails the corresponding test (per `main.md`: stale docs are a bug)
