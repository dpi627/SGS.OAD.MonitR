using HostMonitor.ViewModels;

namespace HostMonitor.Messages;

/// <summary>
/// Message to request opening the settings dialog.
/// </summary>
public sealed class OpenSettingsDialogMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSettingsDialogMessage"/> class.
    /// </summary>
    public OpenSettingsDialogMessage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    /// <summary>
    /// Gets the view model to use for the dialog.
    /// </summary>
    public SettingsViewModel ViewModel { get; }
}
