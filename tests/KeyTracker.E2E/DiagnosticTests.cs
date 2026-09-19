using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

public sealed class DiagnosticTests : IDisposable
{
    private readonly AppFixture _fixture = new();
    private readonly List<string> _lines = new();

    private void Log(string s)
    {
        _lines.Add($"{DateTime.Now:HH:mm:ss.fff} {s}");
        File.WriteAllLines(@"C:\Users\andrs\Desktop\Codigos\KeyTracker\e2e-diag.txt", _lines);
    }

    [Fact]
    public void Quit_StepByStep()
    {
        Log("launching");
        _fixture.Launch();
        Log("launched");

        var desktop = _fixture.Automation.GetDesktop();
        var trayWnd = desktop.FindFirstChild(cf => cf.ByClassName("Shell_TrayWnd"));
        Log($"trayWnd: {trayWnd != null}");

        var iconOnBar = trayWnd?.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
            .FirstOrDefault(e => (e.Name ?? "").Contains("KeyTracker", StringComparison.OrdinalIgnoreCase));
        Log($"icon directly on bar: {iconOnBar != null}");

        var chevron = trayWnd!.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
            .FirstOrDefault(e => (e.Name ?? "").Contains("hidden icons", StringComparison.OrdinalIgnoreCase));
        Log($"chevron: {chevron != null}");
        var crect = chevron!.BoundingRectangle;
        Mouse.Click(new System.Drawing.Point(crect.X + crect.Width / 2, crect.Y + crect.Height / 2));
        Thread.Sleep(800);

        var overflow = desktop.FindFirstChild(cf => cf.ByClassName("NotifyIconOverflowWindow"))
            ?? desktop.FindFirstChild(cf => cf.ByClassName("TopLevelWindowForOverflowXamlIsland"));
        Log($"overflow: {overflow != null}");

        var icon = overflow?.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
            .FirstOrDefault(e => (e.Name ?? "").Contains("KeyTracker", StringComparison.OrdinalIgnoreCase));
        Log($"icon in overflow: {icon != null}");
        if (icon == null) { Assert.True(true); return; }

        var rect = icon.BoundingRectangle;
        Log($"icon rect: X={rect.X} Y={rect.Y} W={rect.Width} H={rect.Height}");
        Thread.Sleep(500);
        Mouse.Click(new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), MouseButton.Right);
        Log("right-clicked");
        Thread.Sleep(700);

        var menu = desktop.FindAllChildren()
            .FirstOrDefault(w => (w.Properties.ClassName.ValueOrDefault ?? "").StartsWith("WindowsForms10.Window"));
        Log($"menu: {menu != null}");

        if (menu == null)
        {
            foreach (var top in desktop.FindAllChildren())
            {
                try { Log($"TOP Name='{top.Name}' ClassName='{top.Properties.ClassName.ValueOrDefault}'"); }
                catch { }
            }
        }
        else
        {
            var mrect = menu.BoundingRectangle;
            Log($"menu rect: X={mrect.X} Y={mrect.Y} W={mrect.Width} H={mrect.Height}");
            var quitY = mrect.Y + (int)(mrect.Height * 0.75);
            var quitX = mrect.X + mrect.Width / 2;
            Mouse.Click(new System.Drawing.Point(quitX, quitY));
            Thread.Sleep(2000);
            Log($"HasExited: {_fixture.App.HasExited}");
        }

        Assert.True(true);
    }

    public void Dispose() => _fixture.Dispose();
}
