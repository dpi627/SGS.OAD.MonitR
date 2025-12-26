using HostMonitor.Models;
using HostMonitor.Models.Enums;

namespace HostMonitor.Services.Interfaces;

/// <summary>
/// Defines a monitoring service.
/// </summary>
public interface IMonitorService
{
    /// <summary>
    /// Gets the supported monitor type.
    /// </summary>
    MonitorType SupportedType { get; }

    /// <summary>
    /// Executes a monitoring check.
    /// </summary>
    Task<MonitorResult> CheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken = default);
}
