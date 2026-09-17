using KeyTracker.Core.Models;

namespace KeyTracker.Core.Keyboards;

public sealed class Abnt2Layout : KeyboardLayout
{
    public override string Name => "ABNT2";

    public override IReadOnlyList<Key> Keys { get; } = BuildKeys();

    private static IReadOnlyList<Key> BuildKeys()
    {
        var keys = new List<Key>();

        void Add(int scanCode, int virtualKey, string label, double x, double y, double width = 1, double height = 1)
        {
            keys.Add(new Key
            {
                ScanCode = scanCode,
                VirtualKey = virtualKey,
                Label = label,
                X = x,
                Y = y,
                Width = width,
                Height = height,
            });
        }

        // Row 1 - number row
        Add(0x29, 0xDE, "'", 0, 0);
        Add(0x02, 0x31, "1", 1, 0);
        Add(0x03, 0x32, "2", 2, 0);
        Add(0x04, 0x33, "3", 3, 0);
        Add(0x05, 0x34, "4", 4, 0);
        Add(0x06, 0x35, "5", 5, 0);
        Add(0x07, 0x36, "6", 6, 0);
        Add(0x08, 0x37, "7", 7, 0);
        Add(0x09, 0x38, "8", 8, 0);
        Add(0x0A, 0x39, "9", 9, 0);
        Add(0x0B, 0x30, "0", 10, 0);
        Add(0x0C, 0xBD, "-", 11, 0);
        Add(0x0D, 0xBB, "=", 12, 0);
        Add(0x0E, 0x08, "Backspace", 13, 0, width: 2);

        // Row 2 - QWERTY row
        Add(0x0F, 0x09, "Tab", 0, 1, width: 1.5);
        Add(0x10, 0x51, "Q", 1.5, 1);
        Add(0x11, 0x57, "W", 2.5, 1);
        Add(0x12, 0x45, "E", 3.5, 1);
        Add(0x13, 0x52, "R", 4.5, 1);
        Add(0x14, 0x54, "T", 5.5, 1);
        Add(0x15, 0x59, "Y", 6.5, 1);
        Add(0x16, 0x55, "U", 7.5, 1);
        Add(0x17, 0x49, "I", 8.5, 1);
        Add(0x18, 0x4F, "O", 9.5, 1);
        Add(0x19, 0x50, "P", 10.5, 1);
        Add(0x1A, 0xDB, "´", 11.5, 1);
        Add(0x1B, 0xDD, "[", 12.5, 1);

        // Row 3 - home row
        Add(0x3A, 0x14, "Caps Lock", 0, 2, width: 1.75);
        Add(0x1E, 0x41, "A", 1.75, 2);
        Add(0x1F, 0x53, "S", 2.75, 2);
        Add(0x20, 0x44, "D", 3.75, 2);
        Add(0x21, 0x46, "F", 4.75, 2);
        Add(0x22, 0x47, "G", 5.75, 2);
        Add(0x23, 0x48, "H", 6.75, 2);
        Add(0x24, 0x4A, "J", 7.75, 2);
        Add(0x25, 0x4B, "K", 8.75, 2);
        Add(0x26, 0x4C, "L", 9.75, 2);
        Add(0x27, 0xBA, "Ç", 10.75, 2);
        Add(0x28, 0xDE, "~", 11.75, 2);
        Add(0x1C, 0x0D, "Enter", 12.75, 2, width: 1.75);

        // Row 4 - bottom letter row
        Add(0x2A, 0xA0, "Shift", 0, 3, width: 1.25);
        Add(0x56, 0xE2, "\\", 1.25, 3);
        Add(0x2C, 0x5A, "Z", 2.25, 3);
        Add(0x2D, 0x58, "X", 3.25, 3);
        Add(0x2E, 0x43, "C", 4.25, 3);
        Add(0x2F, 0x56, "V", 5.25, 3);
        Add(0x30, 0x42, "B", 6.25, 3);
        Add(0x31, 0x4E, "N", 7.25, 3);
        Add(0x32, 0x4D, "M", 8.25, 3);
        Add(0x33, 0xBC, ",", 9.25, 3);
        Add(0x34, 0xBE, ".", 10.25, 3);
        Add(0x35, 0xBF, ";", 11.25, 3);
        Add(0x36, 0xA1, "Shift", 12.25, 3, width: 1.75);

        // Row 5 - modifiers and space
        Add(0x1D, 0xA2, "Ctrl", 0, 4, width: 1.25);
        Add(0x5B, 0x5B, "Win", 1.25, 4, width: 1.25);
        Add(0x38, 0xA4, "Alt", 2.5, 4, width: 1.25);
        Add(0x39, 0x20, "Space", 3.75, 4, width: 6.25);

        return keys;
    }
}
