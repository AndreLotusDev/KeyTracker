using System.Collections.Concurrent;

namespace KeyTracker.Core.Tracking;

public sealed class KeyPressTracker
{
    private readonly ConcurrentDictionary<int, long> _counts = new();

    public void RecordKeyPress(int scanCode)
    {
        _counts.AddOrUpdate(scanCode, 1, static (_, count) => count + 1);
    }

    public IReadOnlyDictionary<int, long> Flush()
    {
        var snapshot = new Dictionary<int, long>();

        foreach (var scanCode in _counts.Keys)
        {
            if (_counts.TryRemove(scanCode, out var count))
                snapshot[scanCode] = count;
        }

        return snapshot;
    }
}
