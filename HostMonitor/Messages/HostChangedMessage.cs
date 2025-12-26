using HostMonitor.Models;

namespace HostMonitor.Messages;

/// <summary>
/// Message raised when a host is added or updated.
/// </summary>
public sealed class HostChangedMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HostChangedMessage"/> class.
    /// </summary>
    public HostChangedMessage(Host host, bool isEdit)
    {
        Host = host;
        IsEdit = isEdit;
    }

    /// <summary>
    /// Gets the host.
    /// </summary>
    public Host Host { get; }

    /// <summary>
    /// Gets a value indicating whether the host was edited.
    /// </summary>
    public bool IsEdit { get; }
}
