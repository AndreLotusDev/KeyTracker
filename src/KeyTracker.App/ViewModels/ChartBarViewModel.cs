using System.Windows;

namespace KeyTracker.App.ViewModels;

public sealed class ChartBarViewModel
{
    public ChartBarViewModel(string label, long total, double ratio, bool showLabel)
    {
        Label = label;
        Total = total;
        UpperHeight = new GridLength(1 - ratio, GridUnitType.Star);
        LowerHeight = new GridLength(ratio, GridUnitType.Star);
        AxisLabel = showLabel ? label : "";
        Tooltip = $"{label}: {total:N0}";
    }

    // Also the accessible name of the item, which e2e tests read through UI Automation.
    public override string ToString() => Tooltip;

    public string Label { get; }
    public long Total { get; }
    public string AxisLabel { get; }
    public string Tooltip { get; }
    public GridLength UpperHeight { get; }
    public GridLength LowerHeight { get; }
}
