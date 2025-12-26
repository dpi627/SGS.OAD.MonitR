using System.Threading.Tasks;
using HostMonitor.Models;

namespace HostMonitor.Messages;

/// <summary>
/// Message to request a delete confirmation.
/// </summary>
public sealed class ConfirmDeleteHostMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmDeleteHostMessage"/> class.
    /// </summary>
    public ConfirmDeleteHostMessage(Host host, TaskCompletionSource<bool> completion)
    {
        Host = host;
        Completion = completion;
    }

    /// <summary>
    /// Gets the host to delete.
    /// </summary>
    public Host Host { get; }

    /// <summary>
    /// Gets the completion source for the confirmation result.
    /// </summary>
    public TaskCompletionSource<bool> Completion { get; }
}
