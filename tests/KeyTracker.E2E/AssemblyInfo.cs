using Xunit;

// Each test launches a real KeyTracker.App process and drives the single physical desktop/taskbar
// (and mutates the process-wide KEYTRACKER_DATA_DIR env var before launch), so tests must not run concurrently.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
