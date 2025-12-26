using HostMonitor.ViewModels;

namespace HostMonitor.Messages;

/// <summary>
/// Message to request opening the add/edit host dialog.
/// </summary>
public sealed class OpenAddEditDialogMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAddEditDialogMessage"/> class.
    /// </summary>
    public OpenAddEditDialogMessage(AddEditHostViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    /// <summary>
    /// Gets the view model to use for the dialog.
    /// </summary>
    public AddEditHostViewModel ViewModel { get; }
}
