using KeyTracker.Core.Statistics;
using LiteDB;

namespace KeyTracker.Infrastructure.Database;

public sealed class LiteDbKeyTotalsSource : IKeyTotalsSource
{
    private readonly ILiteDatabase _database;

    public LiteDbKeyTotalsSource(ILiteDatabase database)
    {
        _database = database;
    }

    public IReadOnlyDictionary<string, long> GetKeyTotals(DateOnly from, DateOnly to, string layoutName)
    {
        var fromText = from.ToString("yyyy-MM-dd");
        var toText = to.ToString("yyyy-MM-dd");

        var collection = _database.GetCollection<DailyKeyRecord>(DailyKeyRecordRepository.CollectionName);
        var records = collection.Find(Query.And(
            Query.Between("Date", fromText, toText),
            Query.EQ("Layout", layoutName)));

        var totals = new Dictionary<string, long>();
        foreach (var record in records)
        {
            foreach (var (label, count) in record.Keys)
                totals[label] = totals.GetValueOrDefault(label) + count;
        }

        return totals;
    }
}
