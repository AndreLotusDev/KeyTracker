using KeyTracker.E2E.Infrastructure;

namespace KeyTracker.E2E;

/// <summary>Covers docs/business/tray-icon.md: Flow "Quit from tray".</summary>
public sealed class TrayQuitTests : IDisposable
{
    private readonly AppFixture _fixture = new();

    [Fact]
    public void PickingQuit_TerminatesTheProcess()
    {
        _fixture.Launch();

        TrayIconAutomation.Quit(_fixture.Automation, "KeyTracker");

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!_fixture.App.HasExited && DateTime.UtcNow < deadline)
            Thread.Sleep(200);

        Assert.True(_fixture.App.HasExited);
    }

    public void Dispose() => _fixture.Dispose();
}
