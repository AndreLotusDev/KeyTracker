# 05 - Dashboard UI

WPF interface: dashboard + heatmap, per the `main.md` mockup.

## Scope

- Dashboard View/ViewModel: cards (today / daily average / this month), heatmap, activity chart, export XLSX button
- Heatmap renders from `KeyboardLayout` (task 02), not hardcoded keys — switching layout shouldn't require touching the renderer
- Period selector (today / week / month / year)
- System tray icon with an option to open the dashboard and to quit
- Starts with Windows (registry `HKCU\...\Run` or a shortcut in the Startup folder)

## Acceptance criteria

- Dashboard opens and shows real data from the local db
- Switching the period updates the numbers and the chart
- Exporting XLSX via the button works
- App shows up in the tray and starts with Windows after a reboot
