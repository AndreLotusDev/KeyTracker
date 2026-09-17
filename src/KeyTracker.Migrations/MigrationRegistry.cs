namespace KeyTracker.Migrations;

public static class MigrationRegistry
{
    public static IReadOnlyList<IMigration> All { get; } = new IMigration[]
    {
        new Migration001(),
    };
}
