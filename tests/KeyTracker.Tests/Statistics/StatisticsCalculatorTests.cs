using KeyTracker.Core.Statistics;

namespace KeyTracker.Tests.Statistics;

public class StatisticsCalculatorTests
{
    [Fact]
    public void Summarize_AggregatesTodayWeekMonthYearAndDailyAverage()
    {
        // Wednesday 2026-09-16 -> week starts Monday 2026-09-14
        var source = new FakeDailyTotalsSource(new Dictionary<DateOnly, long>
        {
            [new DateOnly(2026, 1, 1)] = 100,
            [new DateOnly(2026, 9, 1)] = 200,
            [new DateOnly(2026, 9, 14)] = 50,
            [new DateOnly(2026, 9, 15)] = 60,
            [new DateOnly(2026, 9, 16)] = 70,
        });
        var calculator = new StatisticsCalculator(source);

        var summary = calculator.Summarize(new DateOnly(2026, 9, 16));

        Assert.Equal(70, summary.Today);
        Assert.Equal(180, summary.ThisWeek);
        Assert.Equal(380, summary.ThisMonth);
        Assert.Equal(480, summary.ThisYear);
        var daysElapsed = new DateOnly(2026, 9, 16).DayNumber - new DateOnly(2026, 1, 1).DayNumber + 1;
        Assert.Equal(480.0 / daysElapsed, summary.DailyAverage);
    }

    [Fact]
    public void Summarize_WithNoDataForReferenceDate_ReturnsZerosNotThrows()
    {
        var source = new FakeDailyTotalsSource(new Dictionary<DateOnly, long>());
        var calculator = new StatisticsCalculator(source);

        var summary = calculator.Summarize(new DateOnly(2026, 1, 1));

        Assert.Equal(0, summary.Today);
        Assert.Equal(0, summary.ThisWeek);
        Assert.Equal(0, summary.ThisMonth);
        Assert.Equal(0, summary.ThisYear);
        Assert.Equal(0, summary.DailyAverage);
    }

    private sealed class FakeDailyTotalsSource : IDailyTotalsSource
    {
        private readonly Dictionary<DateOnly, long> _totals;

        public FakeDailyTotalsSource(Dictionary<DateOnly, long> totals) => _totals = totals;

        public IReadOnlyList<DailyTotal> GetTotals(DateOnly from, DateOnly to)
        {
            return _totals
                .Where(pair => pair.Key >= from && pair.Key <= to)
                .Select(pair => new DailyTotal(pair.Key, pair.Value))
                .ToList();
        }
    }
}
