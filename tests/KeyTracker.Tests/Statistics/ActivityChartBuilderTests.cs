using KeyTracker.Core.Statistics;

namespace KeyTracker.Tests.Statistics;

public class ActivityChartBuilderTests
{
    [Fact]
    public void Build_Day_ReturnsOneBucketPerDaySortedByDate()
    {
        var dailyTotals = new List<DailyTotal>
        {
            new(new DateOnly(2026, 9, 16), 70),
            new(new DateOnly(2026, 9, 14), 50),
            new(new DateOnly(2026, 9, 15), 60),
        };

        var buckets = ActivityChartBuilder.Build(dailyTotals, ChartGranularity.Day);

        Assert.Equal(3, buckets.Count);
        Assert.Equal("09/14", buckets[0].Label);
        Assert.Equal(50, buckets[0].Total);
        Assert.Equal("09/16", buckets[2].Label);
        Assert.Equal(70, buckets[2].Total);
    }

    [Fact]
    public void Build_Month_SumsDaysWithinTheSameMonth()
    {
        var dailyTotals = new List<DailyTotal>
        {
            new(new DateOnly(2026, 8, 30), 10),
            new(new DateOnly(2026, 9, 1), 20),
            new(new DateOnly(2026, 9, 16), 30),
        };

        var buckets = ActivityChartBuilder.Build(dailyTotals, ChartGranularity.Month);

        Assert.Equal(2, buckets.Count);
        Assert.Equal(10, buckets[0].Total);
        Assert.Equal(50, buckets[1].Total);
    }

    [Fact]
    public void Build_Year_SumsDaysWithinTheSameYear()
    {
        var dailyTotals = new List<DailyTotal>
        {
            new(new DateOnly(2025, 12, 31), 10),
            new(new DateOnly(2026, 1, 1), 20),
            new(new DateOnly(2026, 9, 16), 30),
        };

        var buckets = ActivityChartBuilder.Build(dailyTotals, ChartGranularity.Year);

        Assert.Equal(2, buckets.Count);
        Assert.Equal("2025", buckets[0].Label);
        Assert.Equal(10, buckets[0].Total);
        Assert.Equal("2026", buckets[1].Label);
        Assert.Equal(50, buckets[1].Total);
    }

    [Fact]
    public void RangeFor_Day_CoversLast30Days()
    {
        var (from, to) = ActivityChartBuilder.RangeFor(ChartGranularity.Day, new DateOnly(2026, 9, 16));

        Assert.Equal(new DateOnly(2026, 8, 18), from);
        Assert.Equal(new DateOnly(2026, 9, 16), to);
    }

    [Fact]
    public void RangeFor_Month_CoversLast12Months()
    {
        var (from, to) = ActivityChartBuilder.RangeFor(ChartGranularity.Month, new DateOnly(2026, 9, 16));

        Assert.Equal(new DateOnly(2025, 10, 1), from);
        Assert.Equal(new DateOnly(2026, 9, 16), to);
    }

    [Fact]
    public void RangeFor_Year_CoversLast5Years()
    {
        var (from, to) = ActivityChartBuilder.RangeFor(ChartGranularity.Year, new DateOnly(2026, 9, 16));

        Assert.Equal(new DateOnly(2022, 1, 1), from);
        Assert.Equal(new DateOnly(2026, 9, 16), to);
    }
}
