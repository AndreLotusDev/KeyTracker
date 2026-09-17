# Page: Export XLSX

## Purpose

Let the user save their typing statistics to an Excel file for use outside the app.

## How it works

Triggered by the "Export XLSX" button on the [Dashboard](dashboard.md), regardless of the dashboard's currently selected period/granularity. Always exports year-to-date data: from January 1st of the current year through today.

1. A native "Save As" dialog opens, filtered to `*.xlsx`, defaulting to filename `KeyTracker-{yyyyMMdd}.xlsx` (today's date).
2. If the user cancels the dialog, nothing happens — no file is written, no log entry.
3. If the user confirms, `DashboardExporter.Export` writes a workbook with two sheets:
   - `Daily`: one row per day with recorded data (`Date`, `Total`), ordered ascending by date.
   - `Summary`: `Today`, `This week`, `This month`, `This year`, `Daily average`.
4. The destination directory is created if it doesn't exist. An existing file at the chosen path is overwritten without an in-app confirmation (the OS save dialog itself may already have warned the user).

## Expected flows

### Flow: Successful export
1. User clicks "Export XLSX".
2. User picks a location/filename and confirms.
3. File is written; `ExportStarted` then `ExportCompleted` are logged with the path (see [docs/backend.md](../backend.md#logging-infrastructureloggingloggersetupcs)).

### Flow: Cancelled export
1. User clicks "Export XLSX".
2. User closes/cancels the save dialog.
3. No file is written, no log entry is produced, dashboard state is unchanged.

## Expected results

- The exported `Daily` sheet only contains days that have a record (no zero-filled gaps for days with no tracked keys).
- `Summary` sheet values match what the dashboard would show if the period selector were set to each of Today/Week/Month/Year at the moment of export.
- Exported data always covers the current calendar year to date, independent of the dashboard's selected period at the time of export.

## Functional requirements

- FR1: Export must produce a valid `.xlsx` readable by Excel/LibreOffice/ClosedXML.
- FR2: Export must not depend on the dashboard's currently selected period or chart granularity.
- FR3: Cancelling the save dialog must be a no-op (no partial file, no log entry).

## Non-functional requirements

- NFR1: Export must not freeze the UI for realistic data volumes (a few years of daily records); if this changes, note whether export runs on a background thread.

## Out of scope / known limits

- No format options (CSV, PDF) — XLSX only.
- No per-key breakdown in the export (only daily totals and period summaries).
