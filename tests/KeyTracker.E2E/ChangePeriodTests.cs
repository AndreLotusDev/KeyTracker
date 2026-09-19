using System.Globalization;
using FlaUI.Core.AutomationElements;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flow "Change period (Today / Week / Month / Year)".</summary>
public sealed class ChangePeriodTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    public ChangePeriodTests()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var yearStart = new DateOnly(today.Year, 1, 1);

        _fixture.SeedDay(today, "ABNT2", new Dictionary<string, long> { ["A"] = 50, ["S"] = 30 });
        _fixture.SeedDay(monthStart, "ABNT2", new Dictionary<string, long> { ["D"] = 15 });
        _fixture.SeedDay(yearStart, "ABNT2", new Dictionary<string, long> { ["F"] = 5 });
    }

    [Theory]
    [InlineData("Today", "Today")]
    [InlineData("Week", "This week")]
    [InlineData("Month", "This month")]
    [InlineData("Year", "This year")]
    public void SelectingPeriod_UpdatesLabelAndTotal(string periodOption, string expectedLabel)
    {
        var window = _fixture.Launch();

        var periodCombo = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodComboBox")).AsComboBox();
        periodCombo.Select(periodOption);

        var periodTotalLabel = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalLabelText")).AsLabel();
        Assert.Equal(expectedLabel, periodTotalLabel.Text);

        // Today only ever contains today's data: deterministic regardless of what day the suite runs.
        if (periodOption == "Today")
        {
            var total = ReadNumber(window, "PeriodTotalText");
            Assert.Equal(80, total);
        }
    }

    [Fact]
    public void WideningThePeriod_NeverDecreasesTheTotal()
    {
        var window = _fixture.Launch();
        var periodCombo = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodComboBox")).AsComboBox();

        periodCombo.Select("Today");
        var todayTotal = ReadNumber(window, "PeriodTotalText");

        periodCombo.Select("Week");
        var weekTotal = ReadNumber(window, "PeriodTotalText");

        periodCombo.Select("Month");
        var monthTotal = ReadNumber(window, "PeriodTotalText");

        periodCombo.Select("Year");
        var yearTotal = ReadNumber(window, "PeriodTotalText");

        Assert.Equal(80, todayTotal);
        Assert.True(weekTotal >= todayTotal);
        Assert.True(monthTotal >= weekTotal);
        Assert.True(yearTotal >= monthTotal);
    }

    [Fact]
    public void SelectingPeriod_RequeriesHeatmapWithoutCrashing()
    {
        var window = _fixture.Launch();
        var periodCombo = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodComboBox")).AsComboBox();

        foreach (var option in new[] { "Week", "Month", "Year", "Today" })
        {
            periodCombo.Select(option);
            var heatmap = window.FindFirstDescendant(cf => cf.ByAutomationId("HeatmapItemsControl"));
            Assert.NotNull(heatmap);
        }
    }

    private static long ReadNumber(Window window, string automationId)
    {
        var text = window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)).AsLabel().Text;
        return (long)double.Parse(text, NumberStyles.Number, CultureInfo.CurrentCulture);
    }

    public void Dispose() => _fixture.Dispose();
}
