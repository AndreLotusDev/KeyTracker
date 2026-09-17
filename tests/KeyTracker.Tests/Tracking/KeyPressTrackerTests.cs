using KeyTracker.Core.Tracking;

namespace KeyTracker.Tests.Tracking;

public class KeyPressTrackerTests
{
    [Fact]
    public void Flush_ReturnsAccumulatedCountsPerScanCode()
    {
        var tracker = new KeyPressTracker();

        tracker.RecordKeyPress(30);
        tracker.RecordKeyPress(30);
        tracker.RecordKeyPress(31);

        var snapshot = tracker.Flush();

        Assert.Equal(2, snapshot[30]);
        Assert.Equal(1, snapshot[31]);
    }

    [Fact]
    public void Flush_ResetsCountsSoNextFlushOnlyHasNewPresses()
    {
        var tracker = new KeyPressTracker();
        tracker.RecordKeyPress(30);

        tracker.Flush();
        var second = tracker.Flush();

        Assert.Empty(second);
    }
}
