namespace HostMonitor.Messages;

/// <summary>
/// Message to request closing a dialog host.
/// </summary>
public sealed class CloseDialogMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CloseDialogMessage"/> class.
    /// </summary>
    public CloseDialogMessage(string dialogIdentifier)
    {
        DialogIdentifier = dialogIdentifier;
    }

    /// <summary>
    /// Gets the dialog identifier to close.
    /// </summary>
    public string DialogIdentifier { get; }
}
