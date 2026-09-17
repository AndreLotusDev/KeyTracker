namespace KeyTracker.Core.Statistics;

public interface IDailyTotalsSource
{
    IReadOnlyList<DailyTotal> GetTotals(DateOnly from, DateOnly to);
}
