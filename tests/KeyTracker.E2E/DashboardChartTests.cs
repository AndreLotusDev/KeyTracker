using FlaUI.Core.AutomationElements;
using FlaUI.Core.Patterns;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flows "Inspect a single chart point" and "Small window".</summary>
public sealed class DashboardChartTests : IDisposable
{
    private readonly AppFixture _fixture = new();
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void DayChart_HasOneLabelledPointPerRecordedDay_WithTotalInTooltipText()
    {
        _fixture.SeedDay(_today, "ABNT2", new Dictionary<string, long> { ["A"] = 42 });
        _fixture.SeedDay(_today.AddDays(-1), "ABNT2", new Dictionary<string, long> { ["A"] = 7 });
        _fixture.SeedDay(_today.AddDays(-2), "ABNT2", new Dictionary<string, long> { ["A"] = 1234 });
        var window = _fixture.Launch();

        var bars = window.FindFirstDescendant(cf => cf.ByAutomationId("ChartBarsItemsControl"))!
            .FindAllDescendants()
            .Where(e => (e.Name ?? "").Contains(": "))
            .ToArray();

        Assert.Equal(3, bars.Length);
        Assert.Contains(bars, bar => bar.Name == $"{_today:MM/dd}: 42");
        Assert.Contains(bars, bar => bar.Name == $"{_today.AddDays(-2):MM/dd}: 1,234");
    }

    [Fact]
    public void ContentScrolls_WhenWindowIsSmallerThanContent()
    {
        var window = _fixture.Launch();

        var scroller = window.FindFirstDescendant(cf => cf.ByAutomationId("DashboardScrollViewer"))!;
        Assert.True(scroller.Patterns.Scroll.Pattern.VerticallyScrollable.Value);

        var export = window.FindFirstDescendant(cf => cf.ByAutomationId("ExportButton"))!;
        export.Patterns.ScrollItem.PatternOrDefault?.ScrollIntoView();
        Assert.NotNull(export);
    }

    public void Dispose() => _fixture.Dispose();
}
