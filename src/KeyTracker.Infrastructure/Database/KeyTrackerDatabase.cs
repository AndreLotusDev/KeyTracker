using LiteDB;
using Serilog;

namespace KeyTracker.Infrastructure.Database;

public sealed class KeyTrackerDatabase : IDisposable
{
    public ILiteDatabase Connection { get; }

    public KeyTrackerDatabase(ILogger logger) : this(DatabasePaths.DatabaseFilePath, logger)
    {
    }

    public KeyTrackerDatabase(string databasePath, ILogger logger)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
        Connection = new LiteDatabase(databasePath);
        logger.Information("DatabaseOpened Path={Path}", databasePath);
    }

    public void Dispose() => Connection.Dispose();
}
