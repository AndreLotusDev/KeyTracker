using KeyTracker.Core.Keyboards;
using LiteDB;

namespace KeyTracker.Infrastructure.Database;

public sealed class DailyKeyRecordRepository
{
    public const string CollectionName = "daily_records";

    private readonly ILiteDatabase _database;

    public DailyKeyRecordRepository(ILiteDatabase database)
    {
        _database = database;
    }

    public void AddCounts(DateOnly date, KeyboardLayout layout, IReadOnlyDictionary<int, long> scanCodeCounts)
    {
        if (scanCodeCounts.Count == 0)
            return;

        var labelsByScanCode = layout.Keys
            .GroupBy(key => key.ScanCode)
            .ToDictionary(group => group.Key, group => group.First().Label);

        var collection = _database.GetCollection<DailyKeyRecord>(CollectionName);
        var dateText = date.ToString("yyyy-MM-dd");
        var id = $"{dateText}_{layout.Name}";

        var record = collection.FindById(id) ?? new DailyKeyRecord
        {
            Id = id,
            Date = dateText,
            Layout = layout.Name,
        };

        foreach (var (scanCode, count) in scanCodeCounts)
        {
            if (!labelsByScanCode.TryGetValue(scanCode, out var label))
                continue;

            record.Keys[label] = record.Keys.GetValueOrDefault(label) + count;
            record.Total += count;
        }

        collection.Upsert(record);
    }
}
