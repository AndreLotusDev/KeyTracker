using KeyTracker.Core.Models;

namespace KeyTracker.Core.Keyboards;

public abstract class KeyboardLayout
{
    public abstract string Name { get; }
    public abstract IReadOnlyList<Key> Keys { get; }
}
