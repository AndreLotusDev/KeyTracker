namespace KeyTracker.Infrastructure.Database;

public sealed class DailyKeyRecord
{
    public string Id { get; set; } = "";
    public string Date { get; set; } = "";
    public string Layout { get; set; } = "";
    public Dictionary<string, long> Keys { get; set; } = new();
    public long Total { get; set; }
}
