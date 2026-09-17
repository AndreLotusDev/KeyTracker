namespace KeyTracker.Core.Statistics;

public sealed class StatisticsCalculator
{
    private readonly IDailyTotalsSource _source;

    public StatisticsCalculator(IDailyTotalsSource source)
    {
        _source = source;
    }

    public StatisticsSummary Summarize(DateOnly referenceDate)
    {
        var yearStart = new DateOnly(referenceDate.Year, 1, 1);
        var monthStart = new DateOnly(referenceDate.Year, referenceDate.Month, 1);
        var weekStart = referenceDate.AddDays(-DaysSinceMonday(referenceDate.DayOfWeek));

        var byDate = _source.GetTotals(yearStart, referenceDate)
            .ToDictionary(total => total.Date, total => total.Total);

        var thisYear = Sum(byDate, yearStart, referenceDate);
        var daysElapsedThisYear = referenceDate.DayNumber - yearStart.DayNumber + 1;

        return new StatisticsSummary
        {
            Today = byDate.GetValueOrDefault(referenceDate),
            ThisWeek = Sum(byDate, weekStart, referenceDate),
            ThisMonth = Sum(byDate, monthStart, referenceDate),
            ThisYear = thisYear,
            DailyAverage = daysElapsedThisYear == 0 ? 0 : (double)thisYear / daysElapsedThisYear,
        };
    }

    private static long Sum(Dictionary<DateOnly, long> byDate, DateOnly from, DateOnly to)
    {
        var sum = 0L;
        for (var date = from; date <= to; date = date.AddDays(1))
            sum += byDate.GetValueOrDefault(date);
        return sum;
    }

    private static int DaysSinceMonday(DayOfWeek dayOfWeek) => ((int)dayOfWeek + 6) % 7;
}
