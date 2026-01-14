using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HostMonitor.Models.Enums;

namespace HostMonitor.Models;

/// <summary>
/// Represents a host to be monitored.
/// </summary>
public class Host : ObservableObject
{
    private const int MaxResponseTimeHistoryEntries = 30;

    private Guid id;
    private string name = string.Empty;
    private string hostnameOrIp = string.Empty;
    private string hostname = string.Empty;
    private string? ipAddress;
    private HostType type;
    private List<MonitorMethod> monitorMethods = new();
    private HostStatus currentStatus = HostStatus.Unknown;
    private DateTime? lastCheckTime;
    private double? averageResponseTimeMs;
    private string? lastErrorMessage;
    private ObservableCollection<string> commandLog = new();
    private ObservableCollection<double> responseTimeHistory = new();
    private bool isMonitoringEnabled = true;

    /// <summary>
    /// Gets or sets the host identifier.
    /// </summary>
    public Guid Id
    {
        get => id;
        set => SetProperty(ref id, value);
    }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    /// <summary>
    /// Gets or sets the hostname or IP address.
    /// </summary>
    public string HostnameOrIp
    {
        get => hostnameOrIp;
        set
        {
            if (SetProperty(ref hostnameOrIp, value))
            {
                OnPropertyChanged(nameof(DisplayAddress));
            }
        }
    }

    /// <summary>
    /// Gets or sets the hostname.
    /// </summary>
    public string Hostname
    {
        get => hostname;
        set
        {
            if (SetProperty(ref hostname, value))
            {
                OnPropertyChanged(nameof(DisplayAddress));
            }
        }
    }

    /// <summary>
    /// Gets or sets the optional IP address.
    /// </summary>
    public string? IpAddress
    {
        get => ipAddress;
        set
        {
            if (SetProperty(ref ipAddress, value))
            {
                OnPropertyChanged(nameof(DisplayAddress));
            }
        }
    }

    /// <summary>
    /// Gets the display address for the host list.
    /// </summary>
    public string DisplayAddress
    {
        get
        {
            var hostname = string.IsNullOrWhiteSpace(Hostname) ? HostnameOrIp : Hostname;
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                return hostname;
            }

            return $"{hostname} ({IpAddress})";
        }
    }

    /// <summary>
    /// Gets or sets the host type.
    /// </summary>
    public HostType Type
    {
        get => type;
        set => SetProperty(ref type, value);
    }

    /// <summary>
    /// Gets or sets the monitoring methods.
    /// </summary>
    public List<MonitorMethod> MonitorMethods
    {
        get => monitorMethods;
        set => SetProperty(ref monitorMethods, value);
    }

    /// <summary>
    /// Gets or sets the current status.
    /// </summary>
    public HostStatus CurrentStatus
    {
        get => currentStatus;
        set => SetProperty(ref currentStatus, value);
    }

    /// <summary>
    /// Gets or sets the last check time.
    /// </summary>
    public DateTime? LastCheckTime
    {
        get => lastCheckTime;
        set => SetProperty(ref lastCheckTime, value);
    }

    /// <summary>
    /// Gets or sets the average response time in milliseconds.
    /// </summary>
    public double? AverageResponseTimeMs
    {
        get => averageResponseTimeMs;
        set => SetProperty(ref averageResponseTimeMs, value);
    }

    /// <summary>
    /// Gets or sets the last error message.
    /// </summary>
    public string? LastErrorMessage
    {
        get => lastErrorMessage;
        set => SetProperty(ref lastErrorMessage, value);
    }

    /// <summary>
    /// Gets or sets whether monitoring is enabled for this host.
    /// </summary>
    public bool IsMonitoringEnabled
    {
        get => isMonitoringEnabled;
        set => SetProperty(ref isMonitoringEnabled, value);
    }

    /// <summary>
    /// Gets the recent command log entries.
    /// </summary>
    public ObservableCollection<string> CommandLog => commandLog;

    /// <summary>
    /// Gets the response time history for charting.
    /// </summary>
    public ObservableCollection<double> ResponseTimeHistory => responseTimeHistory;

    /// <summary>
    /// Adds a response time entry to the history.
    /// </summary>
    public void AddResponseTime(double responseTimeMs)
    {
        ResponseTimeHistory.Add(responseTimeMs);
        while (ResponseTimeHistory.Count > MaxResponseTimeHistoryEntries)
        {
            ResponseTimeHistory.RemoveAt(0);
        }
    }
}
