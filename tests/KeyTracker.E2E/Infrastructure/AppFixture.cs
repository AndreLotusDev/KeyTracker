using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using KeyTracker.Infrastructure.Database;
using LiteDB;

namespace KeyTracker.E2E.Infrastructure;

/// <summary>
/// Launches a real KeyTracker.App process against an isolated, disposable temp database
/// (via KEYTRACKER_DATA_DIR, see DatabasePaths) so each test run is deterministic.
/// </summary>
public sealed class AppFixture : IDisposable
{
    private readonly string _dataDirectory;
    private Application? _app;

    public AppFixture()
    {
        _dataDirectory = Path.Combine(Path.GetTempPath(), $"KeyTrackerE2E_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dataDirectory);
        Automation = new UIA3Automation();
    }

    public UIA3Automation Automation { get; }

    public Application App => _app ?? throw new InvalidOperationException("App has not been launched yet.");

    private string DatabasePath => Path.Combine(_dataDirectory, "keytracker.db");

    /// <summary>Merges the given key counts into the record for the given day (mirrors DailyKeyRecordRepository.AddCounts).</summary>
    public void SeedDay(DateOnly date, string layout, IReadOnlyDictionary<string, long> keysByLabel)
    {
        using var db = new LiteDatabase(DatabasePath);
        var collection = db.GetCollection<DailyKeyRecord>(DailyKeyRecordRepository.CollectionName);
        var dateText = date.ToString("yyyy-MM-dd");
        var id = $"{dateText}_{layout}";

        var record = collection.FindById(id) ?? new DailyKeyRecord { Id = id, Date = dateText, Layout = layout };
        foreach (var (label, count) in keysByLabel)
            record.Keys[label] = record.Keys.GetValueOrDefault(label) + count;
        record.Total = record.Keys.Values.Sum();

        collection.Upsert(record);
    }

    public Window Launch()
    {
        var running = System.Diagnostics.Process.GetProcessesByName("KeyTracker");
        if (running.Length > 0)
            throw new InvalidOperationException(
                "Another KeyTracker instance is running (its tray icon breaks tray automation). Quit it before running e2e tests.");

        Environment.SetEnvironmentVariable("KEYTRACKER_DATA_DIR", _dataDirectory);
        var exePath = Path.ChangeExtension(typeof(KeyTracker.App.App).Assembly.Location, ".exe");
        _app = Application.Launch(exePath);
        var window = WaitForMainWindow();
        window.Focus();
        Thread.Sleep(300); // let the window finish activating before automation interacts with it
        return window;
    }

    public Window WaitForMainWindow(int timeoutSeconds = 20) =>
        App.GetMainWindow(Automation, TimeSpan.FromSeconds(timeoutSeconds))
            ?? throw new InvalidOperationException("KeyTracker main window did not appear within the timeout.");

    public void Dispose()
    {
        try
        {
            // Dismiss any stray tray context menu/overflow flyout left open by a failed
            // assertion, so it doesn't interfere with the next test's tray interactions.
            Keyboard.Press(VirtualKeyShort.ESCAPE);
        }
        catch
        {
            // best effort
        }

        try
        {
            if (_app is { HasExited: false })
                _app.Kill();
        }
        catch
        {
            // best effort process cleanup
        }

        Automation.Dispose();
        _app?.Dispose();

        try
        {
            Directory.Delete(_dataDirectory, recursive: true);
        }
        catch
        {
            // best effort file cleanup
        }
    }
}
