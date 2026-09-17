namespace KeyTracker.Core.Statistics;

public sealed class StatisticsSummary
{
    public long Today { get; init; }
    public long ThisWeek { get; init; }
    public long ThisMonth { get; init; }
    public long ThisYear { get; init; }
    public double DailyAverage { get; init; }
}
