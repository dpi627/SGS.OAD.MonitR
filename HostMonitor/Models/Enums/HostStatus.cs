namespace HostMonitor.Models.Enums;

/// <summary>
/// Represents the current host status.
/// </summary>
public enum HostStatus
{
    /// <summary>
    /// Status has not been determined yet.
    /// </summary>
    Unknown,

    /// <summary>
    /// Host is reachable and healthy.
    /// </summary>
    Online,

    /// <summary>
    /// Host is unreachable.
    /// </summary>
    Offline,

    /// <summary>
    /// Some checks failed.
    /// </summary>
    Warning,

    /// <summary>
    /// Host is currently being checked.
    /// </summary>
    Checking
}
