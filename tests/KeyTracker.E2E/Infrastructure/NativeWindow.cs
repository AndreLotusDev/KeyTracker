using System.Runtime.InteropServices;

namespace KeyTracker.E2E.Infrastructure;

/// <summary>
/// WPF's Window.Hide() clears WS_VISIBLE but the HWND (and its UIA element) can remain
/// discoverable, so ground-truth visibility is checked directly via Win32 instead of UIA.
/// </summary>
internal static class NativeWindow
{
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    public static bool IsVisible(IntPtr handle) => IsWindowVisible(handle);
}
