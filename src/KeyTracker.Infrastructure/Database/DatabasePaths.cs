namespace KeyTracker.Infrastructure.Database;

public static class DatabasePaths
{
    public static string DataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "KeyTracker", "data");

    public static string DatabaseFilePath => Path.Combine(DataDirectory, "keytracker.db");

    public static string BackupDirectory => Path.Combine(DataDirectory, "backup");
}
