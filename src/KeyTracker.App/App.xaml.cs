using System.Windows;
using KeyTracker.Infrastructure.Logging;
using Serilog;

namespace KeyTracker.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Logger = LoggerSetup.CreateLogger();
        Log.Information("ApplicationStarted");

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
