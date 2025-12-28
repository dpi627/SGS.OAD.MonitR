using System;
using WpfApplication = System.Windows.Application;
using MaterialDesignThemes.Wpf;

namespace HostMonitor.Services;

/// <summary>
/// Provides snackbar notifications.
/// </summary>
public sealed class NotificationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationService"/> class.
    /// </summary>
    public NotificationService()
    {
        var dispatcher = WpfApplication.Current?.Dispatcher;
        MessageQueue = dispatcher is null
            ? new SnackbarMessageQueue(TimeSpan.FromSeconds(3))
            : new SnackbarMessageQueue(TimeSpan.FromSeconds(3), dispatcher);
    }

    /// <summary>
    /// Raised when a notification is shown.
    /// </summary>
    public event EventHandler<NotificationEventArgs>? NotificationRaised;

    /// <summary>
    /// Gets the snackbar message queue.
    /// </summary>
    public ISnackbarMessageQueue MessageQueue { get; }

    /// <summary>
    /// Shows a success notification.
    /// </summary>
    public void ShowSuccess(string message)
    {
        Enqueue(NotificationKind.Success, message);
    }

    /// <summary>
    /// Shows an error notification.
    /// </summary>
    public void ShowError(string message)
    {
        Enqueue(NotificationKind.Error, message);
    }

    /// <summary>
    /// Shows a warning notification.
    /// </summary>
    public void ShowWarning(string message)
    {
        Enqueue(NotificationKind.Warning, message);
    }

    private void Enqueue(NotificationKind kind, string message)
    {
        void EnqueueCore()
        {
            MessageQueue.Enqueue(message);
            NotificationRaised?.Invoke(this, new NotificationEventArgs(kind, message));
        }

        var dispatcher = WpfApplication.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
        {
            EnqueueCore();
        }
        else
        {
            dispatcher.BeginInvoke((Action)EnqueueCore);
        }
    }
}
