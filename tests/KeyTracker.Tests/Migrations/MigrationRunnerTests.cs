using KeyTracker.Migrations;
using LiteDB;
using Serilog;

namespace KeyTracker.Tests.Migrations;

public class MigrationRunnerTests : IDisposable
{
    private readonly string _dbPath;
    private readonly string _backupDir;

    public MigrationRunnerTests()
    {
        var root = Path.Combine(Path.GetTempPath(), "KeyTrackerTests_" + Guid.NewGuid());
        Directory.CreateDirectory(root);
        _dbPath = Path.Combine(root, "test.db");
        _backupDir = Path.Combine(root, "backups");
    }

    public void Dispose()
    {
        var root = Path.GetDirectoryName(_dbPath)!;
        if (Directory.Exists(root))
            Directory.Delete(root, recursive: true);
    }

    [Fact]
    public void Run_AppliesPendingMigrationsAndRecordsSchemaVersion()
    {
        var migration = new FakeMigration(version: 1);
        var runner = new MigrationRunner(_dbPath, _backupDir, Log.Logger, new IMigration[] { migration });

        runner.Run();

        Assert.Equal(1, migration.ApplyCount);
        using var db = new LiteDatabase(_dbPath);
        var schema = db.GetCollection<SchemaInfo>("schema_info").FindById(1);
        Assert.Equal(1, schema!.Version);
    }

    [Fact]
    public void Run_IsIdempotent_AlreadyAppliedMigrationsAreNotReapplied()
    {
        var migration = new FakeMigration(version: 1);
        var runner = new MigrationRunner(_dbPath, _backupDir, Log.Logger, new IMigration[] { migration });

        runner.Run();
        runner.Run();

        Assert.Equal(1, migration.ApplyCount);
    }

    [Fact]
    public void Run_BacksUpDatabaseBeforeApplyingDestructiveMigration()
    {
        // Create an initial (non-destructive) version so the db file exists.
        var initial = new FakeMigration(version: 1, isDestructive: false);
        var runner = new MigrationRunner(_dbPath, _backupDir, Log.Logger, new IMigration[] { initial });
        runner.Run();

        var destructive = new FakeMigration(version: 2, isDestructive: true);
        var runner2 = new MigrationRunner(_dbPath, _backupDir, Log.Logger, new IMigration[] { initial, destructive });
        runner2.Run();

        var backupFile = Path.Combine(_backupDir, "keytracker-2.db");
        Assert.True(File.Exists(backupFile));
    }

    private sealed class FakeMigration : IMigration
    {
        public FakeMigration(int version, bool isDestructive = false)
        {
            Version = version;
            IsDestructive = isDestructive;
        }

        public int Version { get; }
        public bool IsDestructive { get; }
        public int ApplyCount { get; private set; }

        public void Apply(ILiteDatabase database) => ApplyCount++;
    }
}
