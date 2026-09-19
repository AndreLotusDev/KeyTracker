using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/tray-icon.md: Flow "Open dashboard from tray".</summary>
public sealed class TrayOpenDashboardTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    [Fact]
    public void DoubleClickingTheTrayIcon_ReopensTheSameWindowInstance()
    {
        var window = _fixture.Launch();
        var originalHandle = window.Properties.NativeWindowHandle.Value;

        window.Close();
        Thread.Sleep(500);
        Assert.False(NativeWindow.IsVisible(originalHandle));

        TrayIconAutomation.OpenDashboard(_fixture.Automation, "KeyTracker");

        var reopened = _fixture.WaitForMainWindow();
        Assert.True(NativeWindow.IsVisible(reopened.Properties.NativeWindowHandle.Value));
        Assert.Equal(originalHandle, reopened.Properties.NativeWindowHandle.Value);
    }

    public void Dispose() => _fixture.Dispose();
}
