# 05 - Dashboard UI

WPF interface: dashboard + heatmap, per the `main.md` mockup.

## Scope

- [x] Dashboard View/ViewModel: cards (today / daily average / this month), heatmap, activity chart, export XLSX button
- [x] Heatmap renders from `KeyboardLayout` (task 02), not hardcoded keys — switching layout shouldn't require touching the renderer
- [x] Period selector (today / week / month / year)
- [x] System tray icon with an option to open the dashboard and to quit
- [x] Starts with Windows (registry `HKCU\...\Run` or a shortcut in the Startup folder)

## Acceptance criteria

- [x] Dashboard opens and shows real data from the local db
- [x] Switching the period updates the numbers and the chart
- [x] Exporting XLSX via the button works
- [x] App shows up in the tray and starts with Windows after a reboot
