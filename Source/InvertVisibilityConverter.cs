using System.Globalization;

namespace GlobalIntelligence.Utils.Converters;

public class InvertVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        => value is not true;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        => value is not true;
}
