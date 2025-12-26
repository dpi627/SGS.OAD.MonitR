using System.Collections.ObjectModel;
using HostMonitor.Models;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.Services;

/// <summary>
/// Provides in-memory host storage.
/// </summary>
public class HostDataService : IHostDataService
{
    private readonly ObservableCollection<Host> _hosts;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostDataService"/> class.
    /// </summary>
    public HostDataService()
    {
        _hosts = new ObservableCollection<Host>();
    }

    /// <inheritdoc />
    public ObservableCollection<Host> GetAllHosts() => _hosts;

    /// <inheritdoc />
    public Host GetHostById(Guid id)
    {
        var host = _hosts.FirstOrDefault(h => h.Id == id);
        if (host is null)
        {
            throw new InvalidOperationException($"Host not found: {id}");
        }

        return host;
    }

    /// <inheritdoc />
    public void AddHost(Host host)
    {
        if (host.Id == Guid.Empty)
        {
            host.Id = Guid.NewGuid();
        }

        _hosts.Add(host);
    }

    /// <inheritdoc />
    public void UpdateHost(Host host)
    {
        var existing = _hosts.FirstOrDefault(h => h.Id == host.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Host not found: {host.Id}");
        }

        existing.Name = host.Name;
        existing.HostnameOrIp = host.HostnameOrIp;
        existing.Type = host.Type;
        existing.MonitorMethods = host.MonitorMethods;
        existing.CurrentStatus = host.CurrentStatus;
        existing.LastCheckTime = host.LastCheckTime;
        existing.AverageResponseTimeMs = host.AverageResponseTimeMs;
        existing.LastErrorMessage = host.LastErrorMessage;
    }

    /// <inheritdoc />
    public void DeleteHost(Guid id)
    {
        var existing = _hosts.FirstOrDefault(h => h.Id == id);
        if (existing is null)
        {
            return;
        }

        _hosts.Remove(existing);
    }
}
