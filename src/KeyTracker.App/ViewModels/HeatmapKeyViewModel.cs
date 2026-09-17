using KeyTracker.Core.Models;

namespace KeyTracker.App.ViewModels;

public sealed class HeatmapKeyViewModel
{
    private const double CellSize = 48;
    private const double Gap = 4;

    public HeatmapKeyViewModel(Key key, long count, double intensity)
    {
        Label = key.Label;
        Count = count;
        Intensity = intensity;
        Left = key.X * CellSize;
        Top = key.Y * CellSize;
        RenderWidth = key.Width * CellSize - Gap;
        RenderHeight = key.Height * CellSize - Gap;
    }

    public string Label { get; }
    public long Count { get; }
    public double Intensity { get; }
    public double Left { get; }
    public double Top { get; }
    public double RenderWidth { get; }
    public double RenderHeight { get; }
}
