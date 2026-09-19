using ClosedXML.Excel;
using FlaUI.Core.AutomationElements;
using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/export-xlsx.md: Flows "Successful export" and "Cancelled export".</summary>
public sealed class ExportXlsxTests : IDisposable
{
    private readonly AppFixture _fixture = new();
    private readonly string _exportDirectory = Path.Combine(Path.GetTempPath(), $"KeyTrackerE2EExport_{Guid.NewGuid():N}");

    public ExportXlsxTests()
    {
        Directory.CreateDirectory(_exportDirectory);

        var today = DateOnly.FromDateTime(DateTime.Now);
        var yearStart = new DateOnly(today.Year, 1, 1);

        _fixture.SeedDay(today, "ABNT2", new Dictionary<string, long> { ["A"] = 50, ["S"] = 30 });
        if (yearStart != today)
            _fixture.SeedDay(yearStart, "ABNT2", new Dictionary<string, long> { ["F"] = 5 });
    }

    [Fact]
    public void ConfirmingTheSaveDialog_WritesAWorkbookWithDailyAndSummarySheets()
    {
        var window = _fixture.Launch();
        var filePath = Path.Combine(_exportDirectory, "export-success.xlsx");

        window.Focus();
        window.FindFirstDescendant(cf => cf.ByAutomationId("ExportButton")).AsButton().Invoke();
        var dialog = SaveDialogAutomation.WaitForDialog(_fixture.Automation, _fixture.App.ProcessId);
        SaveDialogAutomation.SaveAs(dialog, filePath);

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!File.Exists(filePath) && DateTime.UtcNow < deadline)
            Thread.Sleep(200);

        Assert.True(File.Exists(filePath));

        using var workbook = new XLWorkbook(filePath);
        var dailySheet = workbook.Worksheet("Daily");
        var summarySheet = workbook.Worksheet("Summary");

        var today = DateOnly.FromDateTime(DateTime.Now);
        var todayRow = dailySheet.RowsUsed()
            .Skip(1)
            .FirstOrDefault(row => row.Cell(1).GetString() == today.ToString("yyyy-MM-dd"));
        Assert.NotNull(todayRow);
        Assert.Equal(80, todayRow!.Cell(2).GetValue<long>());

        var summaryRows = summarySheet.RowsUsed().Skip(1)
            .ToDictionary(row => row.Cell(1).GetString(), row => row.Cell(2).GetValue<double>());
        Assert.Equal(80, summaryRows["Today"]);
    }

    [Fact]
    public void CancellingTheSaveDialog_WritesNoFileAndLeavesDashboardUnchanged()
    {
        var window = _fixture.Launch();
        var periodTotalBefore = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel().Text;
        var filePath = Path.Combine(_exportDirectory, "export-cancelled.xlsx");

        window.Focus();
        window.FindFirstDescendant(cf => cf.ByAutomationId("ExportButton")).AsButton().Invoke();
        var dialog = SaveDialogAutomation.WaitForDialog(_fixture.Automation, _fixture.App.ProcessId);
        SaveDialogAutomation.Cancel(dialog);

        Thread.Sleep(500);

        Assert.False(File.Exists(filePath));
        Assert.False(Directory.EnumerateFiles(_exportDirectory).Any());

        var periodTotalAfter = window.FindFirstDescendant(cf => cf.ByAutomationId("PeriodTotalText")).AsLabel().Text;
        Assert.Equal(periodTotalBefore, periodTotalAfter);
    }

    public void Dispose()
    {
        _fixture.Dispose();
        try
        {
            Directory.Delete(_exportDirectory, recursive: true);
        }
        catch
        {
            // best effort cleanup
        }
    }
}
