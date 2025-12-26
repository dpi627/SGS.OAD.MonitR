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
    private string hostnameOrIp = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private HostType selectedHostType = HostType.WindowsPC;

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
        HostnameOrIp = host.HostnameOrIp;
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

        var host = new Host
        {
            Id = _editingHostId ?? Guid.NewGuid(),
            Name = Name.Trim(),
            HostnameOrIp = HostnameOrIp.Trim(),
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
        Reset();
    }

    [RelayCommand]
    private void Cancel()
    {
        Reset();
    }

    private bool CanSave()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(HostnameOrIp))
        {
            return false;
        }

        if (!IsValidHost(HostnameOrIp))
        {
            return false;
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

    private static bool IsValidHost(string value)
    {
        var trimmed = value.Trim();
        if (IPAddress.TryParse(trimmed, out _))
        {
            return true;
        }

        var hostType = Uri.CheckHostName(trimmed);
        return hostType == UriHostNameType.Dns
            || hostType == UriHostNameType.IPv4
            || hostType == UriHostNameType.IPv6;
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
        HostnameOrIp = string.Empty;
        SelectedHostType = HostType.WindowsPC;
        EnablePingMonitor = true;
        EnableTcpMonitor = false;
        TcpPorts = new ObservableCollection<int> { 80 };
        NewPort = 443;
        IsEditMode = false;
        _editingHostId = null;
        SaveCommand.NotifyCanExecuteChanged();
    }
}
