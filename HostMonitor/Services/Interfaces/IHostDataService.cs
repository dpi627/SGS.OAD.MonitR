using System.Collections.ObjectModel;
using HostMonitor.Models;

namespace HostMonitor.Services.Interfaces;

/// <summary>
/// Defines host data operations.
/// </summary>
public interface IHostDataService
{
    /// <summary>
    /// Gets all hosts.
    /// </summary>
    ObservableCollection<Host> GetAllHosts();

    /// <summary>
    /// Gets a host by identifier.
    /// </summary>
    Host GetHostById(Guid id);

    /// <summary>
    /// Adds a host.
    /// </summary>
    void AddHost(Host host);

    /// <summary>
    /// Updates a host.
    /// </summary>
    void UpdateHost(Host host);

    /// <summary>
    /// Deletes a host by identifier.
    /// </summary>
    void DeleteHost(Guid id);
}
