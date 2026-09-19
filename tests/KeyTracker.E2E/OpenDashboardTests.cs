using FlaUI.Core.AutomationElements;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flow "Open dashboard", and the zero-data expected result.</summary>
public sealed class OpenDashboardTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    [Fact]
    public void AppStart_ShowsWindowWithTodayPeriodAndZeroTotals()
    {
        var window = _fixture.Launch();

        Assert.False(window.Properties.IsOffscreen.ValueOrDefault);

        var periodCombo = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodComboBox")).AsComboBox();
        Assert.Equal("Today", periodCombo.SelectedItem.Text);

        var periodTotalLabel = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalLabelText")).AsLabel();
        Assert.Equal("Today", periodTotalLabel.Text);

        var periodTotal = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel();
        Assert.Equal("0", periodTotal.Text);

        var dailyAverage = window.FindFirstDescendant(cf => cf.ByAutomationId("DailyAverageText")).AsLabel();
        Assert.Equal("0", dailyAverage.Text);

        var thisMonthTotal = window.FindFirstDescendant(cf => cf.ByAutomationId("ThisMonthTotalText")).AsLabel();
        Assert.Equal("0", thisMonthTotal.Text);

        var heatmap = window.FindFirstDescendant(cf => cf.ByAutomationId("HeatmapItemsControl"));
        Assert.NotNull(heatmap);
        Assert.True(heatmap!.FindAllChildren().Length > 0);

        var exportButton = window.FindFirstDescendant(cf => cf.ByAutomationId("ExportButton")).AsButton();
        Assert.True(exportButton.IsEnabled);
    }

    public void Dispose() => _fixture.Dispose();
}
