using Microsoft.Win32;

namespace KeyTracker.Infrastructure.Windows;

public static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "KeyTracker";

    public static void Enable(string executablePath)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        key?.SetValue(ValueName, $"\"{executablePath}\"");
    }
}
