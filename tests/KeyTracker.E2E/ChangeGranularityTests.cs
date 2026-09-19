using FlaUI.Core.AutomationElements;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flow "Change chart granularity (Day / Week / Month)".</summary>
public sealed class ChangeGranularityTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    public ChangeGranularityTests()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        _fixture.SeedDay(today, "ABNT2", new Dictionary<string, long> { ["A"] = 42 });
    }

    [Theory]
    [InlineData("Month")]
    [InlineData("Year")]
    [InlineData("Day")]
    public void SelectingGranularity_LeavesCardsAndHeatmapUntouched(string granularityOption)
    {
        var window = _fixture.Launch();

        var periodTotalBefore = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel().Text;
        var dailyAverageBefore = window.FindFirstDescendant(cf => cf.ByAutomationId("DailyAverageText")).AsLabel().Text;
        var thisMonthBefore = window.FindFirstDescendant(cf => cf.ByAutomationId("ThisMonthTotalText")).AsLabel().Text;

        var granularityCombo = window.FindFirstDescendant(cf => cf.ByAutomationId("ChartGranularityComboBox")).AsComboBox();
        granularityCombo.Select(granularityOption);

        var periodTotalAfter = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel().Text;
        var dailyAverageAfter = window.FindFirstDescendant(cf => cf.ByAutomationId("DailyAverageText")).AsLabel().Text;
        var thisMonthAfter = window.FindFirstDescendant(cf => cf.ByAutomationId("ThisMonthTotalText")).AsLabel().Text;

        Assert.Equal(periodTotalBefore, periodTotalAfter);
        Assert.Equal(dailyAverageBefore, dailyAverageAfter);
        Assert.Equal(thisMonthBefore, thisMonthAfter);
    }

    public void Dispose() => _fixture.Dispose();
}
