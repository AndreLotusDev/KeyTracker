namespace KeyTracker.Core.Statistics;

public static class PeriodRange
{
    public static (DateOnly From, DateOnly To) For(Period period, DateOnly referenceDate) => period switch
    {
        Period.Today => (referenceDate, referenceDate),
        Period.Week => (referenceDate.AddDays(-DaysSinceMonday(referenceDate.DayOfWeek)), referenceDate),
        Period.Month => (new DateOnly(referenceDate.Year, referenceDate.Month, 1), referenceDate),
        Period.Year => (new DateOnly(referenceDate.Year, 1, 1), referenceDate),
        _ => throw new ArgumentOutOfRangeException(nameof(period)),
    };

    private static int DaysSinceMonday(DayOfWeek dayOfWeek) => ((int)dayOfWeek + 6) % 7;
}
