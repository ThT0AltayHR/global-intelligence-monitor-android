using System.Globalization;

namespace GlobalIntelligence.Utils.Converters;

public class MultiValueConverter{1..20} : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo? culture)
    {
        return values?.Length > 0 ? values[0] : null;
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo? culture)
    {
        throw new NotImplementedException();
    }
}
