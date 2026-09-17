using KeyTracker.Infrastructure.Database;
using LiteDB;

namespace KeyTracker.Migrations;

public sealed class Migration001 : IMigration
{
    public int Version => 1;
    public bool IsDestructive => false;

    public void Apply(ILiteDatabase database)
    {
        var collection = database.GetCollection(DailyKeyRecordRepository.CollectionName);
        collection.EnsureIndex("Date");
        collection.EnsureIndex("Layout");
    }
}
