using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using KeyTracker.Core.Keyboards;
using KeyTracker.Core.Statistics;
using KeyTracker.Infrastructure.Export;
using Microsoft.Win32;

namespace KeyTracker.App.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private const double ChartWidth = 600;
    private const double ChartHeight = 140;

    private readonly StatisticsCalculator _statisticsCalculator;
    private readonly IDailyTotalsSource _dailyTotalsSource;
    private readonly IKeyTotalsSource _keyTotalsSource;
    private readonly DashboardExporter _exporter;
    private readonly KeyboardLayout _layout;

    private Period _selectedPeriod = Period.Today;
    private ChartGranularity _selectedChartGranularity = ChartGranularity.Day;
    private long _periodTotal;
    private double _dailyAverage;
    private long _thisMonthTotal;
    private PointCollection _chartPoints = new();
    private IReadOnlyList<ChartBarViewModel> _chartBars = Array.Empty<ChartBarViewModel>();

    public DashboardViewModel(
        StatisticsCalculator statisticsCalculator,
        IDailyTotalsSource dailyTotalsSource,
        IKeyTotalsSource keyTotalsSource,
        DashboardExporter exporter,
        KeyboardLayout layout)
    {
        _statisticsCalculator = statisticsCalculator;
        _dailyTotalsSource = dailyTotalsSource;
        _keyTotalsSource = keyTotalsSource;
        _exporter = exporter;
        _layout = layout;

        ExportCommand = new RelayCommand(Export);
        Refresh();
    }

    public IReadOnlyList<Period> Periods { get; } = Enum.GetValues<Period>();

    public Period SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            if (_selectedPeriod == value)
                return;

            _selectedPeriod = value;
            OnPropertyChanged();
            Refresh();
        }
    }

    public IReadOnlyList<ChartGranularity> ChartGranularities { get; } = Enum.GetValues<ChartGranularity>();

    public ChartGranularity SelectedChartGranularity
    {
        get => _selectedChartGranularity;
        set
        {
            if (_selectedChartGranularity == value)
                return;

            _selectedChartGranularity = value;
            OnPropertyChanged();
            RefreshChart();
        }
    }

    public string PeriodTotalLabel => SelectedPeriod switch
    {
        Period.Today => "Today",
        Period.Week => "This week",
        Period.Month => "This month",
        Period.Year => "This year",
        _ => "",
    };

    public long PeriodTotal
    {
        get => _periodTotal;
        private set { _periodTotal = value; OnPropertyChanged(); }
    }

    public double DailyAverage
    {
        get => _dailyAverage;
        private set { _dailyAverage = value; OnPropertyChanged(); }
    }

    public long ThisMonthTotal
    {
        get => _thisMonthTotal;
        private set { _thisMonthTotal = value; OnPropertyChanged(); }
    }

    public PointCollection ChartPoints
    {
        get => _chartPoints;
        private set { _chartPoints = value; OnPropertyChanged(); }
    }

    public IReadOnlyList<ChartBarViewModel> ChartBars
    {
        get => _chartBars;
        private set { _chartBars = value; OnPropertyChanged(); }
    }

    public ObservableCollection<HeatmapKeyViewModel> HeatmapKeys { get; } = new();

    public double HeatmapWidth { get; private set; }
    public double HeatmapHeight { get; private set; }

    public ICommand ExportCommand { get; }

    public void Refresh()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var summary = _statisticsCalculator.Summarize(today);

        PeriodTotal = SelectedPeriod switch
        {
            Period.Today => summary.Today,
            Period.Week => summary.ThisWeek,
            Period.Month => summary.ThisMonth,
            Period.Year => summary.ThisYear,
            _ => 0,
        };
        DailyAverage = summary.DailyAverage;
        ThisMonthTotal = summary.ThisMonth;
        OnPropertyChanged(nameof(PeriodTotalLabel));

        var (from, to) = PeriodRange.For(SelectedPeriod, today);

        RefreshChart();

        var keyTotals = _keyTotalsSource.GetKeyTotals(from, to, _layout.Name);
        var maxCount = keyTotals.Count == 0 ? 0 : keyTotals.Values.Max();

        HeatmapKeys.Clear();
        foreach (var key in _layout.Keys)
        {
            var count = keyTotals.GetValueOrDefault(key.Label);
            var intensity = maxCount == 0 ? 0 : (double)count / maxCount;
            HeatmapKeys.Add(new HeatmapKeyViewModel(key, count, intensity));
        }

        HeatmapWidth = _layout.Keys.Count == 0 ? 0 : _layout.Keys.Max(key => (key.X + key.Width)) * 48;
        HeatmapHeight = _layout.Keys.Count == 0 ? 0 : _layout.Keys.Max(key => (key.Y + key.Height)) * 48;
        OnPropertyChanged(nameof(HeatmapWidth));
        OnPropertyChanged(nameof(HeatmapHeight));
    }

    private void RefreshChart()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var (from, to) = ActivityChartBuilder.RangeFor(SelectedChartGranularity, today);

        var dailyTotals = FillMissingDays(_dailyTotalsSource.GetTotals(from, to), from, to);
        var buckets = ActivityChartBuilder.Build(dailyTotals, SelectedChartGranularity);

        ChartPoints = BuildChartPoints(buckets);
        ChartBars = BuildChartBars(buckets);
    }

    // Storage only holds days with typing; without zero days, month/year buckets are missing or misaligned.
    private static IReadOnlyList<DailyTotal> FillMissingDays(IReadOnlyList<DailyTotal> totals, DateOnly from, DateOnly to)
    {
        var byDate = totals.ToDictionary(total => total.Date, total => total.Total);
        var days = new List<DailyTotal>();
        for (var day = from; day <= to; day = day.AddDays(1))
            days.Add(new DailyTotal(day, byDate.GetValueOrDefault(day)));
        return days;
    }

    private static IReadOnlyList<ChartBarViewModel> BuildChartBars(IReadOnlyList<ChartBucket> buckets)
    {
        const int maxLabels = 10;
        var step = buckets.Count <= maxLabels ? 1 : (int)Math.Ceiling(buckets.Count / (double)maxLabels);
        var max = buckets.Count == 0 ? 0 : buckets.Max(bucket => bucket.Total);

        return buckets
            .Select((bucket, index) => new ChartBarViewModel(
                bucket.Label,
                bucket.Total,
                max == 0 ? 0 : (double)bucket.Total / max,
                index % step == 0))
            .ToList();
    }

    private static PointCollection BuildChartPoints(IReadOnlyList<ChartBucket> buckets)
    {
        var points = new PointCollection();
        if (buckets.Count == 0)
            return points;

        var max = buckets.Max(bucket => bucket.Total);
        var count = buckets.Count;

        for (var i = 0; i < count; i++)
        {
            // Center of each bar column, so points line up with the hover markers and labels.
            var x = ChartWidth * (i + 0.5) / count;
            var y = max == 0 ? ChartHeight : ChartHeight - ChartHeight * buckets[i].Total / max;
            points.Add(new Point(x, y));
        }

        return points;
    }

    private void Export()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Excel Workbook|*.xlsx",
            FileName = $"KeyTracker-{DateTime.Now:yyyyMMdd}.xlsx",
        };

        if (dialog.ShowDialog() != true)
            return;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var yearStart = new DateOnly(today.Year, 1, 1);
        var dailyTotals = _dailyTotalsSource.GetTotals(yearStart, today);
        var summary = _statisticsCalculator.Summarize(today);

        _exporter.Export(dailyTotals, summary, dialog.FileName);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
