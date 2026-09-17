using KeyTracker.Core.Keyboards;
using KeyTracker.Core.Models;

namespace KeyTracker.Tests.Keyboards;

public class KeyboardLayoutTests
{
    [Fact]
    public void Abnt2Layout_ExposesNameAndNonEmptyKeySet()
    {
        var layout = new Abnt2Layout();

        Assert.Equal("ABNT2", layout.Name);
        Assert.NotEmpty(layout.Keys);
    }

    [Fact]
    public void Abnt2Layout_HasUniqueScanCodesPerKey()
    {
        var layout = new Abnt2Layout();

        var duplicates = layout.Keys
            .GroupBy(key => key.ScanCode)
            .Where(group => group.Count() > 1)
            .ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void EmptyLayout_HasNoKeys()
    {
        var layout = new EmptyLayout();

        Assert.Empty(layout.Keys);
    }

    private sealed class EmptyLayout : KeyboardLayout
    {
        public override string Name => "Empty";
        public override IReadOnlyList<Key> Keys { get; } = Array.Empty<Key>();
    }
}
