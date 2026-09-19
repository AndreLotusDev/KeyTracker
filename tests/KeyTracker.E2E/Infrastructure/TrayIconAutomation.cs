using System.Drawing;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

namespace KeyTracker.E2E.Infrastructure;

/// <summary>
/// Drives the real Windows taskbar notification area, since KeyTracker's tray icon is a
/// native System.Windows.Forms.NotifyIcon (and its ContextMenuStrip a WinForms ToolStripDropDown),
/// not a WPF/UIA-friendly control. On this machine the icon is collapsed into the "Show hidden
/// icons" overflow flyout, and the ContextMenuStrip exposes no UIA children, so menu items are
/// clicked by position rather than found by name. Windows' own overflow flyout is known to be
/// flaky under UI automation (it can auto-dismiss mid-interaction), so each whole interaction
/// is retried a few times from scratch rather than only retrying individual lookups.
/// </summary>
public static class TrayIconAutomation
{
    public static void OpenDashboard(AutomationBase automation, string iconName) =>
        RetrySequence(() =>
        {
            var icon = FindIcon(automation, iconName);
            Mouse.DoubleClick(Center(icon));
            return true;
        });

    /// <summary>Right-clicks the tray icon and clicks "Quit" (the 2nd of 2 stacked context menu items).</summary>
    public static void Quit(AutomationBase automation, string iconName) =>
        RetrySequence(() =>
        {
            var icon = FindIcon(automation, iconName);
            Thread.Sleep(500);
            Mouse.Click(Center(icon), MouseButton.Right);
            Thread.Sleep(700);

            var menu = WaitFor(() => automation.GetDesktop().FindAllChildren()
                .FirstOrDefault(w => (w.Properties.ClassName.ValueOrDefault ?? "").StartsWith("WindowsForms10.Window")),
                TimeSpan.FromSeconds(5));
            if (menu == null)
                return false;

            var rect = menu.BoundingRectangle;
            var quitPoint = new Point(rect.X + rect.Width / 2, rect.Y + (int)(rect.Height * 0.75));
            Mouse.Click(quitPoint);
            return true;
        });

    private static void RetrySequence(Func<bool> attempt)
    {
        Exception? lastError = null;
        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (attempt())
                    return;
            }
            catch (Exception ex)
            {
                lastError = ex;
            }

            Thread.Sleep(500);
        }

        throw lastError ?? new InvalidOperationException("Tray icon interaction did not complete after retries.");
    }

    private static AutomationElement FindIcon(AutomationBase automation, string iconName)
    {
        var desktop = automation.GetDesktop();
        var trayWnd = desktop.FindFirstChild(cf => cf.ByClassName("Shell_TrayWnd"))
            ?? throw new InvalidOperationException("Taskbar (Shell_TrayWnd) was not found.");

        var icon = FindByNameContains(trayWnd, iconName);
        if (icon != null)
            return icon;

        // The icon is likely collapsed into the "Show hidden icons" overflow flyout.
        var chevron = trayWnd.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
            .FirstOrDefault(e => (e.Name ?? "").Contains("hidden icons", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Tray icon '{iconName}' was not found, and no overflow chevron exists either.");

        var chevronPoint = Center(chevron);
        Mouse.Click(chevronPoint);
        Thread.Sleep(800);
        var overflow = desktop.FindFirstChild(cf => cf.ByClassName("NotifyIconOverflowWindow"))
            ?? desktop.FindFirstChild(cf => cf.ByClassName("TopLevelWindowForOverflowXamlIsland"))
            ?? WaitFor(() =>
            {
                Mouse.Click(chevronPoint);
                return desktop.FindFirstChild(cf => cf.ByClassName("NotifyIconOverflowWindow"))
                    ?? desktop.FindFirstChild(cf => cf.ByClassName("TopLevelWindowForOverflowXamlIsland"));
            }, TimeSpan.FromSeconds(4), retryDelayMs: 500);

        icon = overflow != null
            ? FindByNameContains(overflow, iconName)
            : null;

        return icon ?? throw new InvalidOperationException($"Tray icon '{iconName}' was not found.");
    }

    private static AutomationElement? FindByNameContains(AutomationElement? root, string iconName)
    {
        if (root == null)
            return null;

        return root.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
            .FirstOrDefault(e => (e.Name ?? "").Contains(iconName, StringComparison.OrdinalIgnoreCase));
    }

    private static T? WaitFor<T>(Func<T?> probe, TimeSpan timeout, int retryDelayMs = 300) where T : class
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        T? result = null;
        while (result == null && DateTime.UtcNow < deadline)
        {
            result = probe();
            if (result == null)
                Thread.Sleep(retryDelayMs);
        }
        return result;
    }

    private static Point Center(AutomationElement element)
    {
        var rect = element.BoundingRectangle;
        return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }
}
