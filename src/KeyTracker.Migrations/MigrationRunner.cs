using LiteDB;
using Serilog;

namespace KeyTracker.Migrations;

public sealed class MigrationRunner
{
    private const string SchemaCollectionName = "schema_info";
    private const int SchemaInfoId = 1;

    private readonly string _databasePath;
    private readonly string _backupDirectory;
    private readonly ILogger _logger;
    private readonly IReadOnlyList<IMigration> _migrations;

    public MigrationRunner(string databasePath, string backupDirectory, ILogger logger, IReadOnlyList<IMigration> migrations)
    {
        _databasePath = databasePath;
        _backupDirectory = backupDirectory;
        _logger = logger;
        _migrations = migrations.OrderBy(migration => migration.Version).ToList();
    }

    public void Run()
    {
        var currentVersion = ReadCurrentVersion();
        var pending = _migrations.Where(migration => migration.Version > currentVersion).ToList();

        foreach (var migration in pending)
        {
            _logger.Information("MigrationStarted Version={Version}", migration.Version);

            if (migration.IsDestructive)
                BackupDatabase(migration.Version);

            using (var db = new LiteDatabase(_databasePath))
            {
                db.BeginTrans();
                try
                {
                    migration.Apply(db);
                    WriteVersion(db, migration.Version);
                    db.Commit();
                }
                catch
                {
                    db.Rollback();
                    throw;
                }
            }

            _logger.Information("MigrationCompleted Version={Version}", migration.Version);
        }
    }

    private int ReadCurrentVersion()
    {
        if (!File.Exists(_databasePath))
            return 0;

        using var db = new LiteDatabase(_databasePath);
        var collection = db.GetCollection<SchemaInfo>(SchemaCollectionName);
        return collection.FindById(SchemaInfoId)?.Version ?? 0;
    }

    private void BackupDatabase(int version)
    {
        if (!File.Exists(_databasePath))
            return;

        Directory.CreateDirectory(_backupDirectory);
        var backupPath = Path.Combine(_backupDirectory, $"keytracker-{version}.db");
        File.Copy(_databasePath, backupPath, overwrite: true);
    }

    private static void WriteVersion(ILiteDatabase db, int version)
    {
        var collection = db.GetCollection<SchemaInfo>(SchemaCollectionName);
        collection.Upsert(new SchemaInfo { Id = SchemaInfoId, Version = version });
    }
}
