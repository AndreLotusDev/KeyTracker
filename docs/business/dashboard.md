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

The chart draws one point marker per bucket (for Day granularity: one per day that has recorded keys). Hovering a marker's column shows a tooltip `"<label>: <total>"` (e.g. `09/19: 1,234`) for that single bucket. The x-axis shows at most ~10 labels (every Nth bucket when there are more); the tooltip always carries the full label.

The window content scrolls vertically when the window is too small to show everything (heatmap, chart, and Export button stay reachable). On tall windows the chart grows to fill the free space.

The dashboard reloads (pending key presses are flushed to the database first, then cards, heatmap, and chart are re-read):
- whenever the window is activated (gains focus, including when re-opened from the tray)
- every 10 seconds while the window is visible and not minimized

Data source: LiteDB via `IDailyTotalsSource` / `IKeyTotalsSource` (see [docs/backend.md](../backend.md)). No caching — every reload re-reads the database.

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
2. Only `ChartPoints` and `ChartBars` update; cards and heatmap are untouched.

### Flow: Inspect a single chart point
1. User hovers a point on the chart line.
2. A tooltip shows that bucket's label and total (`"<label>: <total>"`).

### Flow: Refresh on focus / periodically
1. User returns to the window (focus, or "Open Dashboard" from the tray), or 10 seconds pass while it is visible.
2. Pending keys are flushed and the dashboard re-reads all data; cards, heatmap, and chart show the latest totals for the currently selected period and granularity.

### Flow: Small window
1. User resizes the window smaller than the content.
2. A vertical scrollbar appears; the chart and Export button are reachable by scrolling.

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

- No manual refresh button — data is re-read on period/granularity change, window activation, and the 10 s timer.
- No date range picker beyond the four fixed periods.
