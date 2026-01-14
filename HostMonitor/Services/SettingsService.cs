using CommunityToolkit.Mvvm.ComponentModel;

namespace HostMonitor.Services;

/// <summary>
/// Service for managing application settings.
/// </summary>
public partial class SettingsService : ObservableObject
{
    private const int DefaultIntervalSeconds = 5;
    private const int MinIntervalSeconds = 1;
    private const int MaxIntervalSeconds = 3600;

    [ObservableProperty]
    private int monitorIntervalSeconds = DefaultIntervalSeconds;

    /// <summary>
    /// Gets the minimum interval in seconds.
    /// </summary>
    public static int MinInterval => MinIntervalSeconds;

    /// <summary>
    /// Gets the maximum interval in seconds.
    /// </summary>
    public static int MaxInterval => MaxIntervalSeconds;

    /// <summary>
    /// Validates and sets the monitor interval.
    /// </summary>
    public bool TrySetInterval(int seconds)
    {
        if (seconds < MinIntervalSeconds || seconds > MaxIntervalSeconds)
        {
            return false;
        }

        MonitorIntervalSeconds = seconds;
        return true;
    }
}
