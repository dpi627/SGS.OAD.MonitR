namespace HostMonitor.Services.Monitoring;

/// <summary>
/// Provides data for issued monitor commands.
/// </summary>
public sealed class MonitorCommandEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MonitorCommandEventArgs"/> class.
    /// </summary>
    public MonitorCommandEventArgs(Guid hostId, string command, DateTime timestamp)
    {
        HostId = hostId;
        Command = command;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Gets the host identifier.
    /// </summary>
    public Guid HostId { get; }

    /// <summary>
    /// Gets the command text.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the timestamp for the command.
    /// </summary>
    public DateTime Timestamp { get; }
}
