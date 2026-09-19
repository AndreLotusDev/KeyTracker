# Frontend

Covers `KeyTracker.App` (WPF): `Views/`, `ViewModels/`.

## Structure

- Single window app: `Views/MainWindow.xaml` + `MainWindow.xaml.cs`. There is no navigation/routing — see the dashboard doc in [docs/business/dashboard.md](business/dashboard.md) for the one screen.
- `ViewModels/DashboardViewModel.cs` is the sole view model, built with a hand-rolled MVVM (`INotifyPropertyChanged`, `RelayCommand`), no framework (no Prism/MVVM Toolkit).
- `ViewModels/HeatmapKeyViewModel.cs` wraps a single `Key` (from `Core.Keyboards`) with render position/size and computed `Intensity` for one heatmap cell.
- `ViewModels/ChartBarViewModel.cs` is one chart bucket: label, total, tooltip text, and star row heights that place its marker.
- `Views/IntensityToBrushConverter.cs` is a `IValueConverter` mapping `Intensity` (0..1) to a heatmap color brush.

## Composition root

`App.xaml.cs: OnStartup` wires everything by hand (no DI container): creates the database, migration runner, tracker, hook, statistics sources, exporter, and finally `DashboardViewModel`, then assigns it as `MainWindow.DataContext`. Any new dependency the view model needs must be constructed and passed in here.

## Window lifecycle

- Closing the window does not exit the app: `MainWindow.Closing` is intercepted, cancelled, and the window is hidden (`App.xaml.cs`). Only the tray icon's "Quit" exits the process.
- The tray icon's "Open Dashboard" (or double-click) re-shows and activates the same `MainWindow` instance — it is never recreated.

- `MainWindow.Activated` and a 10 s `DispatcherTimer` (only while visible and not minimized) call `Reload()` in `App.xaml.cs`, which flushes `PeriodicFlushService` and then calls the public `DashboardViewModel.Refresh()`.

## Layout

- The window content sits in a vertical `ScrollViewer`; the inner grid's `MinHeight` is bound to the viewport height, so the chart row fills free space on large windows and content scrolls on small ones.
- The chart is a `Polyline` (`ChartPoints`) overlaid with one column per `ChartBarViewModel` (`ChartBars`): a transparent hover area with a `Tooltip` and an ellipse marker at the bucket's height. Axis labels come from `ChartBarViewModel.AxisLabel` (empty for skipped buckets).

## Data binding

- All dashboard values (`PeriodTotal`, `DailyAverage`, `ThisMonthTotal`, `ChartPoints`, `ChartBars`, `HeatmapKeys`, `HeatmapWidth/Height`) are bound one-way from `DashboardViewModel` to XAML.
- `SelectedPeriod` and `SelectedChartGranularity` are two-way bound `ComboBox` selections that trigger `Refresh()` / `RefreshChart()` on change.
- `ExportCommand` is bound to the "Export XLSX" button.

## Non-functional requirements

- UI must stay responsive: `Refresh()` runs on the UI thread synchronously against LiteDB reads, so query cost must stay low enough not to block rendering (see [docs/backend.md](backend.md)).
- No hardcoded per-key layout in XAML or view models — the heatmap renders from `KeyboardLayout`/`Key` (see [docs/backend.md](backend.md#statistics-corestatistics)) so a new layout requires no UI changes.

## Keep this doc in sync

Update this file whenever `Views/`, `ViewModels/`, or the composition root in `App.xaml.cs` changes — new screens, new bindings, or a different lifecycle/DI approach.
