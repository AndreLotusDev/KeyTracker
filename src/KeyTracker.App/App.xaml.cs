using System.IO;
using System.Windows;
using System.Windows.Threading;
using KeyTracker.App.ViewModels;
using KeyTracker.App.Views;
using KeyTracker.Core.Keyboards;
using KeyTracker.Core.Statistics;
using KeyTracker.Core.Tracking;
using KeyTracker.Infrastructure.Database;
using KeyTracker.Infrastructure.Export;
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

    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(10);

    private DispatcherTimer? _refreshTimer;
    private KeyboardHook? _keyboardHook;
    private KeyPressTracker? _keyTracker;
    private KeyTrackerDatabase? _database;
    private PeriodicFlushService? _flushService;
    private TrayIconService? _trayIcon;
    private MainWindow? _mainWindow;

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

        var dailyTotalsSource = new LiteDbDailyTotalsSource(_database.Connection);
        var keyTotalsSource = new LiteDbKeyTotalsSource(_database.Connection);
        var statisticsCalculator = new StatisticsCalculator(dailyTotalsSource);
        var exporter = new DashboardExporter(Log.Logger);
        var viewModel = new DashboardViewModel(statisticsCalculator, dailyTotalsSource, keyTotalsSource, exporter, layout);

        _mainWindow = new MainWindow { DataContext = viewModel };
        _mainWindow.Closing += (_, args) =>
        {
            args.Cancel = true;
            _mainWindow.Hide();
        };
        _mainWindow.Show();

        // Flush pending key presses first so the dashboard shows up-to-date numbers.
        void Reload()
        {
            _flushService.Flush();
            viewModel.Refresh();
        }

        _mainWindow.Activated += (_, _) => Reload();
        _refreshTimer = new DispatcherTimer { Interval = RefreshInterval };
        _refreshTimer.Tick += (_, _) =>
        {
            if (_mainWindow.IsVisible && _mainWindow.WindowState != WindowState.Minimized)
                Reload();
        };
        _refreshTimer.Start();

        var icon = Environment.ProcessPath is { } processPath
            ? System.Drawing.Icon.ExtractAssociatedIcon(processPath)
            : null;
        _trayIcon = new TrayIconService(icon ?? System.Drawing.SystemIcons.Application);
        _trayIcon.OpenRequested += () =>
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        };
        _trayIcon.ExitRequested += Shutdown;

        // Skip touching the real Windows startup registration when running under e2e tests
        // (signaled by KEYTRACKER_DATA_DIR pointing at an isolated, disposable data directory).
        if (Environment.GetEnvironmentVariable("KEYTRACKER_DATA_DIR") is null && Environment.ProcessPath is { } exePath)
            StartupRegistration.Enable(exePath);

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _refreshTimer?.Stop();
        _flushService?.Flush();
        _flushService?.Dispose();
        _keyboardHook?.Dispose();
        _trayIcon?.Dispose();
        _database?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
