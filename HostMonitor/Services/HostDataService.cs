using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.Services;

/// <summary>
/// Provides host storage with local persistence.
/// </summary>
public class HostDataService : IHostDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly ObservableCollection<Host> _hosts;
    private readonly string _storagePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostDataService"/> class.
    /// </summary>
    public HostDataService()
    {
        _storagePath = GetStoragePath();
        _hosts = LoadHosts();
        var hasStoredHosts = _hosts.Count > 0;
        AddDefaultHost();
        if (!hasStoredHosts && _hosts.Count > 0)
        {
            SaveHosts();
        }
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
        SaveHosts();
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
        existing.Hostname = host.Hostname;
        existing.IpAddress = host.IpAddress;
        existing.Type = host.Type;
        existing.MonitorMethods = host.MonitorMethods ?? new List<MonitorMethod>();
        existing.CurrentStatus = host.CurrentStatus;
        existing.LastCheckTime = host.LastCheckTime;
        existing.AverageResponseTimeMs = host.AverageResponseTimeMs;
        existing.LastErrorMessage = host.LastErrorMessage;
        SaveHosts();
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
        SaveHosts();
    }

    private void AddDefaultHost()
    {
        if (_hosts.Count > 0)
        {
            return;
        }

        var localhost = new Host
        {
            Id = Guid.NewGuid(),
            Name = "本機電腦",
            Hostname = "localhost",
            HostnameOrIp = "localhost",
            IpAddress = "127.0.0.1",
            Type = HostType.PC,
            MonitorMethods = new List<MonitorMethod>
            {
                new MonitorMethod
                {
                    Type = MonitorType.IcmpPing,
                    IsEnabled = true
                }
            }
        };

        _hosts.Add(localhost);
    }

    private ObservableCollection<Host> LoadHosts()
    {
        if (!File.Exists(_storagePath))
        {
            return new ObservableCollection<Host>();
        }

        try
        {
            var json = File.ReadAllText(_storagePath);
            var snapshots = JsonSerializer.Deserialize<List<HostSnapshot>>(json, JsonOptions)
                ?? new List<HostSnapshot>();
            var hosts = snapshots.Select(MapSnapshotToHost).ToList();
            return new ObservableCollection<Host>(hosts);
        }
        catch
        {
            return new ObservableCollection<Host>();
        }
    }

    private void SaveHosts()
    {
        try
        {
            var snapshots = _hosts.Select(MapHostToSnapshot).ToList();
            var json = JsonSerializer.Serialize(snapshots, JsonOptions);
            File.WriteAllText(_storagePath, json);
        }
        catch
        {
        }
    }

    private static Host MapSnapshotToHost(HostSnapshot snapshot)
    {
        var hostname = snapshot.Hostname ?? string.Empty;
        var hostnameOrIp = snapshot.HostnameOrIp ?? string.Empty;
        var resolvedHostname = string.IsNullOrWhiteSpace(hostname) ? hostnameOrIp : hostname;
        var resolvedAddress = string.IsNullOrWhiteSpace(hostnameOrIp) ? resolvedHostname : hostnameOrIp;

        return new Host
        {
            Id = snapshot.Id == Guid.Empty ? Guid.NewGuid() : snapshot.Id,
            Name = snapshot.Name ?? string.Empty,
            Hostname = resolvedHostname,
            HostnameOrIp = resolvedAddress,
            IpAddress = snapshot.IpAddress,
            Type = snapshot.Type,
            MonitorMethods = snapshot.MonitorMethods ?? new List<MonitorMethod>(),
            CurrentStatus = HostStatus.Unknown
        };
    }

    private static HostSnapshot MapHostToSnapshot(Host host)
    {
        return new HostSnapshot
        {
            Id = host.Id,
            Name = host.Name,
            Hostname = host.Hostname,
            HostnameOrIp = host.HostnameOrIp,
            IpAddress = host.IpAddress,
            Type = host.Type,
            MonitorMethods = host.MonitorMethods ?? new List<MonitorMethod>()
        };
    }

    private static string GetStoragePath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HostMonitor");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "hosts.json");
    }

    private sealed class HostSnapshot
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? HostnameOrIp { get; set; }

        public string? Hostname { get; set; }

        public string? IpAddress { get; set; }

        public HostType Type { get; set; }

        public List<MonitorMethod>? MonitorMethods { get; set; }
    }
}
