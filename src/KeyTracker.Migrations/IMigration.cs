using LiteDB;

namespace KeyTracker.Migrations;

public interface IMigration
{
    int Version { get; }
    bool IsDestructive { get; }
    void Apply(ILiteDatabase database);
}
