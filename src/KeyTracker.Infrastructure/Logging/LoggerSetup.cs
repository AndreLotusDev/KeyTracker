using Serilog;

namespace KeyTracker.Infrastructure.Logging;

public static class LoggerSetup {
    public static ILogger CreateLogger() {
        var logsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KeyTracker", "logs");
        Directory.CreateDirectory(logsDirectory);

        return new LoggerConfiguration()
            .WriteTo.File(
                Path.Combine(logsDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
