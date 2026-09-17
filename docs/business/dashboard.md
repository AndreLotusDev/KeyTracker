# Page: Dashboard (Main Window)

## Layout (V1)

```
┌─────────────────────────────────────────────┐
│ KeyTracker                         Today ▼  │
├─────────────────────────────────────────────┤
│                                             │
│  58.392          51.203          1.24 M     │
│  Today           Daily average   This month │
│                                             │
├─────────────────────────────────────────────┤
│                                             │
│             Keyboard Heatmap                │
│                                             │
│      [Q][W][E][R][T][Y][U][I][O][P]         │
│       [A][S][D][F][G][H][J][K][L]           │
│        [Z][X][C][V][B][N][M]                │
│                                             │
├─────────────────────────────────────────────┤
│ Activity                                    │
│                                             │
│  60k ┤       ╭─╮                            │
│  40k ┤ ╭─╮ ╭─╯ ╰─╮                          │
│  20k ┤─╯ ╰─╯     ╰──                        │
│      └────────────────────────────           │
│        M  T  W  T  F  S  S                  │
│                                             │
│                                [Export XLSX]│
└─────────────────────────────────────────────┘
```

## Purpose

Show the user how much they've typed, where on the keyboard, and over time.

## How it works

On open, the window loads the current `SelectedPeriod` (default: Today) and renders:
- three summary cards: period total, daily average, this-month total
- a keyboard heatmap for the selected period, colored by relative key intensity
- an activity line chart for the selected granularity (default: Day), with an "Export XLSX" button

Changing the period dropdown re-queries totals and the heatmap for that period. Changing the chart granularity dropdown re-queries and rebuilds only the chart (heatmap and cards are unaffected).

Data source: LiteDB via `IDailyTotalsSource` / `IKeyTotalsSource` (see [docs/backend.md](../backend.md)). No caching — every dropdown change re-reads the database.

## Expected flows

### Flow: Open dashboard
1. App starts, or user picks "Open Dashboard" from the tray icon.
2. Window becomes visible/activated with data for the last-selected period (defaults to Today on first launch).
3. Cards, heatmap, and chart render without a loading spinner (query is expected to be fast; see NFRs).

### Flow: Change period (Today / Week / Month / Year)
1. User selects a new value in the top-right `ComboBox`.
2. `PeriodTotal`, `PeriodTotalLabel`, `DailyAverage`, `ThisMonthTotal` update.
3. Chart re-ranges to match the new period's date range and rebuilds.
4. Heatmap re-queries key totals for the new period's date range; intensities are re-normalized against the new max.

### Flow: Change chart granularity (Day / Week / Month)
1. User selects a new value in the chart's `ComboBox`.
2. Only `ChartPoints` and `ChartLabels` update; cards and heatmap are untouched.

### Flow: Close window
1. User clicks the window's close button.
2. Window hides; the app keeps running in the tray (process is not terminated).

## Expected results

- A day/period with zero recorded keys shows totals of 0, an empty/flat chart, and an all-zero-intensity heatmap (no crash, no divide-by-zero — `Refresh()` guards `maxCount == 0`).
- Heatmap intensity is always relative to the max key count in the current period/layout, so the busiest key(s) are always at full intensity.
- `PeriodTotalLabel` text always matches the selected period ("Today", "This week", "This month", "This year").

## Functional requirements

- FR1: Support periods Today, Week, Month, Year.
- FR2: Support chart granularities Day, Week, Month.
- FR3: Heatmap must render for whatever `KeyboardLayout` is active without layout-specific UI code.
- FR4: Closing the window must not terminate the app.
- FR5: "Export XLSX" button must be reachable and enabled regardless of the selected period/granularity (export always covers year-to-date, independent of the dashboard's current selection — see [export-xlsx.md](export-xlsx.md)).

## Non-functional requirements

- NFR1: Dropdown changes must feel instant (no visible lag) for realistic data volumes (single user, local LiteDB, up to a few years of daily records).
- NFR2: No hardcoded key layout — heatmap must adapt to any `KeyboardLayout` (ABNT2, ANSI, ISO, Custom).

## Out of scope / known limits

- No manual refresh button — data is only re-read on period/granularity change or app restart.
- No date range picker beyond the four fixed periods.
