# 02 - Keyboard Layout & Tracking

Core of the app: keyboard layout model + key capture via a Windows hook.

## Scope

- `KeyTracker.Core/Models`: `Key` (ScanCode, VirtualKey, Label, X, Y, Width, Height)
- `KeyTracker.Core/Keyboards`: abstract `KeyboardLayout` + `ABNT2` implementation (only this one for now, ANSI/ISO/Custom stay pluggable but not implemented yet)
- `KeyTracker.Infrastructure/Windows`: low-level keyboard hook (`WH_KEYBOARD_LL`) capturing keys globally
- `KeyTracker.Core/Tracking`: service that receives hook events and accumulates counts in memory (by ScanCode)

## Performance (critical)

- The hook callback must return fast (only increment an in-memory counter, no I/O, no heavy locking). A slow hook lags the whole keyboard.
- Disk persistence is task 03's responsibility, not this one — here it just accumulates in memory and exposes a way to "flush" from the outside.

## Acceptance criteria

- Running the app and typing, the in-memory counter reflects the correct keys (verify via test or temporary log)
- Hook doesn't freeze or noticeably delay typing
- Logs `KeyboardHookRegistered` / `KeyboardHookFailed` as appropriate
