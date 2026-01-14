using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        Enqueue(NotificationKind.Success, message, PackIconKind.CheckCircle, "#4CAF50");
    }

    /// <summary>
    /// Shows an error notification.
    /// </summary>
    public void ShowError(string message)
    {
        Enqueue(NotificationKind.Error, message, PackIconKind.AlertCircle, "#F44336");
    }

    /// <summary>
    /// Shows a warning notification.
    /// </summary>
    public void ShowWarning(string message)
    {
        Enqueue(NotificationKind.Warning, message, PackIconKind.Alert, "#FF9800");
    }

    private void Enqueue(NotificationKind kind, string message, PackIconKind iconKind, string colorHex)
    {
        void EnqueueCore()
        {
            var content = CreateNotificationContent(message, iconKind, colorHex);
            MessageQueue.Enqueue(content);
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

    private static object CreateNotificationContent(string message, PackIconKind iconKind, string colorHex)
    {
        var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex);
        var brush = new SolidColorBrush(color);

        var icon = new PackIcon
        {
            Kind = iconKind,
            Foreground = brush,
            Width = 20,
            Height = 20,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };

        var text = new TextBlock
        {
            Text = message,
            VerticalAlignment = VerticalAlignment.Center
        };

        var panel = new StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal
        };
        panel.Children.Add(icon);
        panel.Children.Add(text);

        return panel;
    }
}
