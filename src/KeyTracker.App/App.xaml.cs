using System.IO;
using System.Windows;
using KeyTracker.Core.Keyboards;
using KeyTracker.Core.Tracking;
using KeyTracker.Infrastructure.Database;
using KeyTracker.Infrastructure.Logging;
using KeyTracker.Infrastructure.Windows;
using KeyTracker.Migrations;
using Serilog;

namespace KeyTracker.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(30);

    private KeyboardHook? _keyboardHook;
    private KeyPressTracker? _keyTracker;
    private KeyTrackerDatabase? _database;
    private PeriodicFlushService? _flushService;

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Logger = LoggerSetup.CreateLogger();
        Log.Information("ApplicationStarted");

        Directory.CreateDirectory(DatabasePaths.DataDirectory);
        new MigrationRunner(DatabasePaths.DatabaseFilePath, DatabasePaths.BackupDirectory, Log.Logger, MigrationRegistry.All).Run();

        _database = new KeyTrackerDatabase(Log.Logger);
        var repository = new DailyKeyRecordRepository(_database.Connection);
        var layout = new Abnt2Layout();

        _keyTracker = new KeyPressTracker();
        _keyboardHook = new KeyboardHook(Log.Logger);
        _keyboardHook.KeyDown += (scanCode, _) => _keyTracker.RecordKeyPress(scanCode);
        _keyboardHook.Start();

        _flushService = new PeriodicFlushService(_keyTracker, repository, layout, FlushInterval);

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _flushService?.Flush();
        _flushService?.Dispose();
        _keyboardHook?.Dispose();
        _database?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
