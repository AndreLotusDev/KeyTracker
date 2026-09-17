using KeyTracker.Core.Statistics;
using LiteDB;

namespace KeyTracker.Infrastructure.Database;

public sealed class LiteDbDailyTotalsSource : IDailyTotalsSource
{
    private readonly ILiteDatabase _database;

    public LiteDbDailyTotalsSource(ILiteDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<DailyTotal> GetTotals(DateOnly from, DateOnly to)
    {
        var fromText = from.ToString("yyyy-MM-dd");
        var toText = to.ToString("yyyy-MM-dd");

        var collection = _database.GetCollection<DailyKeyRecord>(DailyKeyRecordRepository.CollectionName);
        var records = collection.Find(Query.Between("Date", fromText, toText));

        return records
            .GroupBy(record => record.Date)
            .Select(group => new DailyTotal(DateOnly.ParseExact(group.Key, "yyyy-MM-dd"), group.Sum(record => record.Total)))
            .OrderBy(total => total.Date)
            .ToList();
    }
}
