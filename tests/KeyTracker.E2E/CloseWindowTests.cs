using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/dashboard.md: Flow "Close window".</summary>
public sealed class CloseWindowTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    [Fact]
    public void ClosingTheWindow_HidesItButKeepsTheProcessRunning()
    {
        var window = _fixture.Launch();
        var handle = window.Properties.NativeWindowHandle.Value;

        window.Close();
        Thread.Sleep(500);

        Assert.False(NativeWindow.IsVisible(handle));
        Assert.False(_fixture.App.HasExited);
    }

    public void Dispose() => _fixture.Dispose();
}
