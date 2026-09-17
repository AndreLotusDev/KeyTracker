# Page: Tray Icon

## Purpose

Let the app run in the background and give the user a way to reopen the dashboard or quit without a taskbar window.

## How it works

A `NotifyIcon` is created on startup and stays visible for the life of the process. It has a right-click context menu ("Open Dashboard", "Quit") and responds to double-click the same as "Open Dashboard".

## Expected flows

### Flow: Open dashboard from tray
1. User double-clicks the tray icon, or right-clicks and picks "Open Dashboard".
2. The existing `MainWindow` instance is shown, its state is set to `Normal` (un-minimized), and it is activated (brought to foreground).

### Flow: Quit from tray
1. User right-clicks and picks "Quit".
2. The app shuts down: pending tracked keystrokes are flushed to the database, the keyboard hook is unhooked, the tray icon is removed, the database connection is closed, and logs are flushed (`App.xaml.cs: OnExit`).

## Expected results

- The tray icon is present immediately after app launch and remains present the entire session — it is never hidden or removed except on quit.
- "Open Dashboard" never creates a second window; it reuses the same instance.
- Quit is the only path that terminates the process — closing the main window ([dashboard.md](dashboard.md)) does not.

## Functional requirements

- FR1: Tray icon must offer both "Open Dashboard" and "Quit".
- FR2: Double-click must behave identically to "Open Dashboard".
- FR3: Quit must flush unsaved tracked keystrokes before the process exits.

## Non-functional requirements

- NFR1: Tray icon and its menu must not block or delay keyboard tracking.

## Out of scope / known limits

- No "pause tracking" option from the tray menu.
- Icon is the app's own executable icon (or a system default fallback if extraction fails) — no per-state icon variants.
