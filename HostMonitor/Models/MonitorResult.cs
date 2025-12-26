using HostMonitor.Models.Enums;

namespace HostMonitor.Models;

/// <summary>
/// Represents a monitoring result.
/// </summary>
public class MonitorResult
{
    /// <summary>
    /// Gets or sets the host identifier.
    /// </summary>
    public Guid HostId { get; set; }

    /// <summary>
    /// Gets or sets the monitor type.
    /// </summary>
    public MonitorType MonitorType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the check succeeded.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the check time.
    /// </summary>
    public DateTime CheckTime { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the TCP port for TCP checks.
    /// </summary>
    public int? Port { get; set; }
}
