namespace HostMonitor.Services;

/// <summary>
/// Event arguments for notification messages.
/// </summary>
public sealed class NotificationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
    /// </summary>
    public NotificationEventArgs(NotificationKind kind, string message)
    {
        Kind = kind;
        Message = message;
    }

    /// <summary>
    /// Gets the notification kind.
    /// </summary>
    public NotificationKind Kind { get; }

    /// <summary>
    /// Gets the notification message.
    /// </summary>
    public string Message { get; }
}
