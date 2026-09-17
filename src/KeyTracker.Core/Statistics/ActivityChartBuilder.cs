namespace KeyTracker.Core.Statistics;

public static class ActivityChartBuilder
{
    public static (DateOnly From, DateOnly To) RangeFor(ChartGranularity granularity, DateOnly referenceDate) => granularity switch
    {
        ChartGranularity.Day => (referenceDate.AddDays(-29), referenceDate),
        ChartGranularity.Month => (new DateOnly(referenceDate.Year, referenceDate.Month, 1).AddMonths(-11), referenceDate),
        ChartGranularity.Year => (new DateOnly(referenceDate.Year, 1, 1).AddYears(-4), referenceDate),
        _ => throw new ArgumentOutOfRangeException(nameof(granularity)),
    };

    public static IReadOnlyList<ChartBucket> Build(IReadOnlyList<DailyTotal> dailyTotals, ChartGranularity granularity) => granularity switch
    {
        ChartGranularity.Day => dailyTotals
            .OrderBy(total => total.Date)
            .Select(total => new ChartBucket(total.Date.ToString("MM/dd"), total.Total))
            .ToList(),

        ChartGranularity.Month => dailyTotals
            .GroupBy(total => new DateOnly(total.Date.Year, total.Date.Month, 1))
            .OrderBy(group => group.Key)
            .Select(group => new ChartBucket(group.Key.ToString("MMM yy"), group.Sum(total => total.Total)))
            .ToList(),

        ChartGranularity.Year => dailyTotals
            .GroupBy(total => total.Date.Year)
            .OrderBy(group => group.Key)
            .Select(group => new ChartBucket(group.Key.ToString(), group.Sum(total => total.Total)))
            .ToList(),

        _ => throw new ArgumentOutOfRangeException(nameof(granularity)),
    };
}
