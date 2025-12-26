using System;
using System.Globalization;
using System.Windows.Data;
using HostMonitor.Models.Enums;

namespace HostMonitor.Converters;

/// <summary>
/// Converts host status to a status color.
/// </summary>
public sealed class StatusToColorConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not HostStatus status)
        {
            return System.Windows.Media.Brushes.Gray;
        }

        var color = status switch
        {
            HostStatus.Online => "#4CAF50",
            HostStatus.Offline => "#F44336",
            HostStatus.Warning => "#FF9800",
            HostStatus.Checking => "#2196F3",
            _ => "#9E9E9E"
        };

        return new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Windows.Data.Binding.DoNothing;
    }
}
