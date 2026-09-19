using FlaUI.Core;
using FlaUI.Core.AutomationElements;

namespace KeyTracker.E2E.Infrastructure;

/// <summary>Drives the native Windows "Save As" common dialog used by DashboardViewModel.Export.</summary>
public static class SaveDialogAutomation
{
    // Well-known control ids for the Vista-style common file dialog.
    private const string FileNameComboBoxId = "1148";
    private const string SaveButtonId = "1";
    private const string CancelButtonId = "2";

    public static Window WaitForDialog(AutomationBase automation, int processId, int timeoutSeconds = 30)
    {
        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        while (DateTime.UtcNow < deadline)
        {
            var window = automation.GetDesktop()
                .FindAllChildren(cf => cf.ByClassName("#32770"))
                .Select(e => e.AsWindow())
                .FirstOrDefault(w => w.Properties.ProcessId.ValueOrDefault == processId);

            if (window != null)
                return window;

            Thread.Sleep(300);
        }

        throw new InvalidOperationException("Save dialog did not appear within the timeout.");
    }

    public static void SaveAs(Window dialog, string filePath)
    {
        var nameBox = dialog.FindFirstDescendant(cf => cf.ByAutomationId(FileNameComboBoxId))
            ?? throw new InvalidOperationException("Save dialog file name box was not found.");
        nameBox.AsComboBox().EditableText = filePath;

        var saveButton = dialog.FindFirstDescendant(cf => cf.ByAutomationId(SaveButtonId))
            ?? throw new InvalidOperationException("Save dialog Save button was not found.");
        saveButton.AsButton().Invoke();
    }

    public static void Cancel(Window dialog)
    {
        var cancelButton = dialog.FindFirstDescendant(cf => cf.ByAutomationId(CancelButtonId))
            ?? throw new InvalidOperationException("Save dialog Cancel button was not found.");
        cancelButton.AsButton().Invoke();
    }
}
