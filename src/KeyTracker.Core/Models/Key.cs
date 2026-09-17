namespace KeyTracker.Core.Models;

public sealed class Key
{
    public required int ScanCode { get; init; }
    public required int VirtualKey { get; init; }
    public required string Label { get; init; }
    public required double X { get; init; }
    public required double Y { get; init; }
    public required double Width { get; init; }
    public required double Height { get; init; }
}
