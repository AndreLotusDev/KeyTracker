namespace KeyTracker.Core.Statistics;

public interface IKeyTotalsSource
{
    IReadOnlyDictionary<string, long> GetKeyTotals(DateOnly from, DateOnly to, string layoutName);
}
