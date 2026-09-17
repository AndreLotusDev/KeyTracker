using System.Windows;
using KeyTracker.Core.Tracking;
using KeyTracker.Infrastructure.Logging;
using KeyTracker.Infrastructure.Windows;
using Serilog;

namespace KeyTracker.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private KeyboardHook? _keyboardHook;
    private KeyPressTracker? _keyTracker;

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Logger = LoggerSetup.CreateLogger();
        Log.Information("ApplicationStarted");

        _keyTracker = new KeyPressTracker();
        _keyboardHook = new KeyboardHook(Log.Logger);
        _keyboardHook.KeyDown += (scanCode, _) => _keyTracker.RecordKeyPress(scanCode);
        _keyboardHook.Start();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _keyboardHook?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
