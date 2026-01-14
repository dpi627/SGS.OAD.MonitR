using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HostMonitor.Messages;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.ViewModels;

/// <summary>
/// View model for adding or editing hosts.
/// </summary>
public partial class AddEditHostViewModel : ObservableObject
{
    private readonly IHostDataService _hostDataService;
    private readonly NotificationService _notificationService;
    private Guid? _editingHostId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string hostname = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string ipAddress = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private HostType selectedHostType = HostType.PC;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool enablePingMonitor = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool enableTcpMonitor;

    [ObservableProperty]
    private ObservableCollection<int> tcpPorts = new() { 80 };

    [ObservableProperty]
    private int newPort = 443;

    [ObservableProperty]
    private bool isEditMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddEditHostViewModel"/> class.
    /// </summary>
    public AddEditHostViewModel(IHostDataService hostDataService, NotificationService notificationService)
    {
        _hostDataService = hostDataService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Loads host information for editing.
    /// </summary>
    public void LoadHost(Host host)
    {
        IsEditMode = true;
        _editingHostId = host.Id;
        Name = host.Name;
        Hostname = string.IsNullOrWhiteSpace(host.Hostname) ? host.HostnameOrIp : host.Hostname;
        IpAddress = host.IpAddress ?? string.Empty;
        SelectedHostType = host.Type;

        EnablePingMonitor = host.MonitorMethods.Any(m => m.Type == MonitorType.IcmpPing && m.IsEnabled);
        var tcpPorts = host.MonitorMethods
            .Where(m => m.Type == MonitorType.TcpPort && m.IsEnabled)
            .Select(m => m.Port)
            .Where(port => port.HasValue)
            .Select(port => port!.Value)
            .ToList();

        EnableTcpMonitor = tcpPorts.Count > 0;
        TcpPorts = new ObservableCollection<int>(tcpPorts.Count > 0 ? tcpPorts : new[] { 80 });
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Resets the form to defaults.
    /// </summary>
    public void ResetForm()
    {
        Reset();
    }

    [RelayCommand]
    private void AddPort()
    {
        if (NewPort < 1 || NewPort > 65535)
        {
            _notificationService.ShowWarning("端口需介於 1 到 65535");
            return;
        }

        if (TcpPorts.Contains(NewPort))
        {
            _notificationService.ShowWarning("端口已存在");
            return;
        }

        TcpPorts.Add(NewPort);
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void RemovePort(int port)
    {
        TcpPorts.Remove(port);
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        if (!CanSave())
        {
            _notificationService.ShowWarning("請確認輸入資料");
            return;
        }

        var trimmedHostname = Hostname.Trim();
        var trimmedIpAddress = string.IsNullOrWhiteSpace(IpAddress) ? null : IpAddress.Trim();
        var monitorAddress = ResolveMonitorAddress(trimmedHostname, trimmedIpAddress);
        var host = new Host
        {
            Id = _editingHostId ?? Guid.NewGuid(),
            Name = Name.Trim(),
            HostnameOrIp = monitorAddress,
            Hostname = trimmedHostname,
            IpAddress = trimmedIpAddress,
            Type = SelectedHostType,
            MonitorMethods = BuildMonitorMethods()
        };

        if (IsEditMode)
        {
            _hostDataService.UpdateHost(host);
        }
        else
        {
            _hostDataService.AddHost(host);
        }

        WeakReferenceMessenger.Default.Send(new HostChangedMessage(host, IsEditMode));
        WeakReferenceMessenger.Default.Send(new CloseDialogMessage("RootDialog"));
        Reset();
    }

    [RelayCommand]
    private void Cancel()
    {
        Reset();
        WeakReferenceMessenger.Default.Send(new CloseDialogMessage("RootDialog"));
    }

    private bool CanSave()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Hostname))
        {
            return false;
        }

        var hostnameValid = IsValidHostname(Hostname);
        if (!hostnameValid)
        {
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                return false;
            }

            if (!IsValidIpAddress(IpAddress))
            {
                return false;
            }
        }

        if (!EnablePingMonitor && !EnableTcpMonitor)
        {
            return false;
        }

        if (EnableTcpMonitor)
        {
            if (TcpPorts.Count == 0)
            {
                return false;
            }

            if (TcpPorts.Any(port => port < 1 || port > 65535))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidHostname(string value)
    {
        var trimmed = value.Trim();
        var hostType = Uri.CheckHostName(trimmed);
        return hostType == UriHostNameType.Dns;
    }

    private static bool IsValidIpAddress(string value)
    {
        var trimmed = value.Trim();
        return IPAddress.TryParse(trimmed, out _);
    }

    private static string ResolveMonitorAddress(string hostname, string? ipAddress)
    {
        if (IsValidHostname(hostname))
        {
            return hostname;
        }

        return ipAddress ?? hostname;
    }

    private List<MonitorMethod> BuildMonitorMethods()
    {
        var methods = new List<MonitorMethod>();

        if (EnablePingMonitor)
        {
            methods.Add(new MonitorMethod { Type = MonitorType.IcmpPing, IsEnabled = true });
        }

        if (EnableTcpMonitor)
        {
            foreach (var port in TcpPorts.Distinct())
            {
                methods.Add(new MonitorMethod
                {
                    Type = MonitorType.TcpPort,
                    IsEnabled = true,
                    Port = port
                });
            }
        }

        return methods;
    }

    private void Reset()
    {
        Name = string.Empty;
        Hostname = string.Empty;
        IpAddress = string.Empty;
        SelectedHostType = HostType.PC;
        EnablePingMonitor = true;
        EnableTcpMonitor = false;
        TcpPorts = new ObservableCollection<int> { 80 };
        NewPort = 443;
        IsEditMode = false;
        _editingHostId = null;
        SaveCommand.NotifyCanExecuteChanged();
    }
}
