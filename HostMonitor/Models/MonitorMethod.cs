using HostMonitor.Models.Enums;

namespace HostMonitor.Models;

/// <summary>
/// Defines a monitoring method for a host.
/// </summary>
public class MonitorMethod
{
    /// <summary>
    /// Gets or sets the monitoring type.
    /// </summary>
    public MonitorType Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this method is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the TCP port for TCP monitoring.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds.
    /// </summary>
    public int TimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the interval between checks in seconds.
    /// </summary>
    public int IntervalSeconds { get; set; } = 60;
}
