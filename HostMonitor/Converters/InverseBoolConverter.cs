using System;
using System.Globalization;
using System.Windows.Data;

namespace HostMonitor.Converters;

/// <summary>
/// Inverts a boolean value.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : System.Windows.Data.Binding.DoNothing;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : System.Windows.Data.Binding.DoNothing;
    }
}
