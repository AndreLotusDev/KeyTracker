using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flow "Refresh on focus / periodically".</summary>
public sealed class DashboardRefreshTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    [Fact]
    public void TypedKeys_AppearOnDashboardWithoutReopening()
    {
        var window = _fixture.Launch();
        var total = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel();
        Assert.Equal("0", total.Text);

        // Letters chosen so ComboBox type-ahead can't change the period (Today/Week/Month/Year).
        // Scan codes 0x1E/0x1F/0x20 = A/S/D; sent as scan codes because that is what the tracker records.
        foreach (ushort scanCode in new ushort[] { 0x1E, 0x1F, 0x20 })
            Keyboard.TypeScanCode(scanCode, false);

        // Timer refreshes every 10 s while the window is visible.
        var deadline = DateTime.UtcNow.AddSeconds(25);
        while (DateTime.UtcNow < deadline && total.Text == "0")
            Thread.Sleep(500);

        Assert.NotEqual("0", total.Text);
    }

    public void Dispose() => _fixture.Dispose();
}
