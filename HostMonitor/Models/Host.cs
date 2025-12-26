using HostMonitor.Models.Enums;

namespace HostMonitor.Models;

/// <summary>
/// Represents a host to be monitored.
/// </summary>
public class Host
{
    /// <summary>
    /// Gets or sets the host identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hostname or IP address.
    /// </summary>
    public string HostnameOrIp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the host type.
    /// </summary>
    public HostType Type { get; set; }

    /// <summary>
    /// Gets or sets the monitoring methods.
    /// </summary>
    public List<MonitorMethod> MonitorMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets the current status.
    /// </summary>
    public HostStatus CurrentStatus { get; set; } = HostStatus.Unknown;

    /// <summary>
    /// Gets or sets the last check time.
    /// </summary>
    public DateTime? LastCheckTime { get; set; }

    /// <summary>
    /// Gets or sets the average response time in milliseconds.
    /// </summary>
    public double? AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the last error message.
    /// </summary>
    public string? LastErrorMessage { get; set; }
}
