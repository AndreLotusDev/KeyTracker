using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KeyTracker.App.Views;

public sealed class IntensityToBrushConverter : IValueConverter
{
    private static readonly Color Cold = Color.FromRgb(0x2A, 0x2D, 0x34);
    private static readonly Color Hot = Color.FromRgb(0xE0, 0x4F, 0x2A);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var intensity = value is double d ? Math.Clamp(d, 0, 1) : 0;

        var color = Color.FromRgb(
            (byte)(Cold.R + (Hot.R - Cold.R) * intensity),
            (byte)(Cold.G + (Hot.G - Cold.G) * intensity),
            (byte)(Cold.B + (Hot.B - Cold.B) * intensity));

        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
