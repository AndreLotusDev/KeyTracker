using KeyTracker.Core.Keyboards;
using KeyTracker.Core.Tracking;

namespace KeyTracker.Infrastructure.Database;

public sealed class PeriodicFlushService : IDisposable
{
    private readonly KeyPressTracker _tracker;
    private readonly DailyKeyRecordRepository _repository;
    private readonly KeyboardLayout _layout;
    private readonly Timer _timer;
    private readonly object _flushLock = new();

    public PeriodicFlushService(KeyPressTracker tracker, DailyKeyRecordRepository repository, KeyboardLayout layout, TimeSpan interval)
    {
        _tracker = tracker;
        _repository = repository;
        _layout = layout;
        _timer = new Timer(_ => Flush(), null, interval, interval);
    }

    public void Flush()
    {
        lock (_flushLock)
        {
            var counts = _tracker.Flush();
            if (counts.Count == 0)
                return;

            _repository.AddCounts(DateOnly.FromDateTime(DateTime.Now), _layout, counts);
        }
    }

    public void Dispose() => _timer.Dispose();
}
