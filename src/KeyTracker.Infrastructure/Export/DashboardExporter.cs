using ClosedXML.Excel;
using KeyTracker.Core.Statistics;
using Serilog;

namespace KeyTracker.Infrastructure.Export;

public sealed class DashboardExporter
{
    private readonly ILogger _logger;

    public DashboardExporter(ILogger logger)
    {
        _logger = logger;
    }

    public void Export(IReadOnlyList<DailyTotal> dailyTotals, StatisticsSummary summary, string filePath)
    {
        _logger.Information("ExportStarted Path={Path}", filePath);

        using (var workbook = new XLWorkbook())
        {
            var dailySheet = workbook.Worksheets.Add("Daily");
            dailySheet.Cell(1, 1).Value = "Date";
            dailySheet.Cell(1, 2).Value = "Total";

            var row = 2;
            foreach (var dailyTotal in dailyTotals.OrderBy(total => total.Date))
            {
                dailySheet.Cell(row, 1).Value = dailyTotal.Date.ToString("yyyy-MM-dd");
                dailySheet.Cell(row, 2).Value = dailyTotal.Total;
                row++;
            }

            var summarySheet = workbook.Worksheets.Add("Summary");
            summarySheet.Cell(1, 1).Value = "Period";
            summarySheet.Cell(1, 2).Value = "Total";
            summarySheet.Cell(2, 1).Value = "Today";
            summarySheet.Cell(2, 2).Value = summary.Today;
            summarySheet.Cell(3, 1).Value = "This week";
            summarySheet.Cell(3, 2).Value = summary.ThisWeek;
            summarySheet.Cell(4, 1).Value = "This month";
            summarySheet.Cell(4, 2).Value = summary.ThisMonth;
            summarySheet.Cell(5, 1).Value = "This year";
            summarySheet.Cell(5, 2).Value = summary.ThisYear;
            summarySheet.Cell(6, 1).Value = "Daily average";
            summarySheet.Cell(6, 2).Value = summary.DailyAverage;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            workbook.SaveAs(filePath);
        }

        _logger.Information("ExportCompleted Path={Path}", filePath);
    }
}
