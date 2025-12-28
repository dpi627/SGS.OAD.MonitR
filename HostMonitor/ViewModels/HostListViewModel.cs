using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HostMonitor.Messages;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services;
using HostMonitor.Services.Interfaces;
using HostMonitor.Services.Monitoring;
using WpfApplication = System.Windows.Application;

namespace HostMonitor.ViewModels;

/// <summary>
/// View model for managing host list operations.
/// </summary>
public partial class HostListViewModel : ObservableObject
{
    private const int MaxCommandLogEntries = 200;

    private readonly IHostDataService _hostDataService;
    private readonly AddEditHostViewModel _addEditHostViewModel;
    private readonly MonitorOrchestrator _orchestrator;
    private readonly NotificationService _notificationService;

    /// <summary>
    /// Gets the host collection.
    /// </summary>
    public ObservableCollection<Host> Hosts { get; }

    [ObservableProperty]
    private Host? selectedHost;

    [ObservableProperty]
    private string? searchText;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostListViewModel"/> class.
    /// </summary>
    public HostListViewModel(
        IHostDataService hostDataService,
        AddEditHostViewModel addEditHostViewModel,
        MonitorOrchestrator orchestrator,
        NotificationService notificationService)
    {
        _hostDataService = hostDataService;
        _addEditHostViewModel = addEditHostViewModel;
        _orchestrator = orchestrator;
        _notificationService = notificationService;
        Hosts = _hostDataService.GetAllHosts();
        _orchestrator.MonitorCommandIssued += OnMonitorCommandIssued;
        _orchestrator.MonitorResultReceived += OnMonitorResultReceived;

        WeakReferenceMessenger.Default.Register<HostChangedMessage>(this, (_, message) =>
        {
            var existing = Hosts.FirstOrDefault(host => host.Id == message.Host.Id);
            if (existing is not null)
            {
                SelectedHost = existing;
                _notificationService.ShowSuccess($"已更新主機: {message.Host.Name}");
                return;
            }

            Hosts.Add(message.Host);
            SelectedHost = message.Host;
            _notificationService.ShowSuccess($"已新增主機: {message.Host.Name}");
        });
    }

    private void OnMonitorCommandIssued(object? sender, MonitorCommandEventArgs args)
    {
        var dispatcher = WpfApplication.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
        {
            AppendCommand(args);
        }
        else
        {
            dispatcher.BeginInvoke(() => AppendCommand(args));
        }
    }

    private void OnMonitorResultReceived(object? sender, MonitorResult result)
    {
        var dispatcher = WpfApplication.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
        {
            AppendResponse(result);
        }
        else
        {
            dispatcher.BeginInvoke(() => AppendResponse(result));
        }
    }

    private void AppendCommand(MonitorCommandEventArgs args)
    {
        var host = Hosts.FirstOrDefault(item => item.Id == args.HostId);
        if (host is null)
        {
            return;
        }

        var line = $"[{args.Timestamp:HH:mm:ss}] {args.Command}";
        AppendLine(host, line);
    }

    private void AppendResponse(MonitorResult result)
    {
        var host = Hosts.FirstOrDefault(item => item.Id == result.HostId);
        if (host is null)
        {
            return;
        }

        var timestamp = result.CheckTime == default ? DateTime.Now : result.CheckTime;
        var status = result.IsSuccess
            ? $"OK {result.ResponseTimeMs:N2} ms"
            : $"FAIL {result.ErrorMessage ?? "Unknown error"}";
        var line = $"[{timestamp:HH:mm:ss}] <- {status}";
        AppendLine(host, line);
    }

    private static void AppendLine(Host host, string line)
    {
        host.CommandLog.Add(line);
        while (host.CommandLog.Count > MaxCommandLogEntries)
        {
            host.CommandLog.RemoveAt(0);
        }
    }

    [RelayCommand]
    private Task AddHostAsync()
    {
        _addEditHostViewModel.ResetForm();
        WeakReferenceMessenger.Default.Send(new OpenAddEditDialogMessage(_addEditHostViewModel));
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task EditHostAsync(Host? host)
    {
        if (host is null)
        {
            return Task.CompletedTask;
        }

        _addEditHostViewModel.LoadHost(host);
        WeakReferenceMessenger.Default.Send(new OpenAddEditDialogMessage(_addEditHostViewModel));
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteHostAsync(Host? host)
    {
        if (host is null)
        {
            return;
        }

        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        WeakReferenceMessenger.Default.Send(new ConfirmDeleteHostMessage(host, completion));

        if (!await completion.Task)
        {
            return;
        }

        _hostDataService.DeleteHost(host.Id);
        _notificationService.ShowWarning($"已刪除主機: {host.Name}");
    }

    [RelayCommand]
    private async Task ManualCheckAsync(Host? host)
    {
        if (host is null)
        {
            return;
        }

        host.CurrentStatus = HostStatus.Checking;

        try
        {
            var results = await _orchestrator.CheckHostAsync(host);
            ApplyResults(host, results);
            foreach (var result in results)
            {
                AppendResponse(result);
            }
        }
        catch (Exception ex)
        {
            host.CurrentStatus = HostStatus.Offline;
            host.LastCheckTime = DateTime.Now;
            host.AverageResponseTimeMs = null;
            host.LastErrorMessage = ex.Message;
        }
    }

    private static void ApplyResults(Host host, List<MonitorResult> results)
    {
        host.LastCheckTime = DateTime.Now;

        if (results.Count == 0)
        {
            host.CurrentStatus = HostStatus.Unknown;
            host.AverageResponseTimeMs = null;
            host.LastErrorMessage = null;
            return;
        }

        var successCount = results.Count(result => result.IsSuccess);
        var failureCount = results.Count - successCount;

        if (successCount == results.Count)
        {
            host.CurrentStatus = HostStatus.Online;
        }
        else if (failureCount == results.Count)
        {
            host.CurrentStatus = HostStatus.Offline;
        }
        else
        {
            host.CurrentStatus = HostStatus.Warning;
        }

        host.AverageResponseTimeMs = results.Average(result => result.ResponseTimeMs);

        var errors = results
            .Where(result => !string.IsNullOrWhiteSpace(result.ErrorMessage))
            .Select(result => result.ErrorMessage)
            .Distinct();
        host.LastErrorMessage = string.Join("; ", errors);
    }
}
