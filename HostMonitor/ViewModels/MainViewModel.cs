using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HostMonitor.Messages;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services;
using HostMonitor.Services.Monitoring;
using MaterialDesignThemes.Wpf;

namespace HostMonitor.ViewModels;

/// <summary>
/// Main window coordinator view model.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private const int OfflineNotificationIntervalSeconds = 30;

    private readonly MonitorOrchestrator _orchestrator;
    private readonly NotificationService _notificationService;
    private readonly Dictionary<Guid, Dictionary<MonitorMethodKey, MonitorResult>> _latestResults = new();
    private CancellationTokenSource? _monitoringCts;
    private System.Threading.Timer? _offlineNotificationTimer;

    [ObservableProperty]
    private HostListViewModel hostListViewModel;

    [ObservableProperty]
    private bool isMonitoring;

    /// <summary>
    /// Gets the application version string.
    /// </summary>
    public string AppVersion { get; }

    /// <summary>
    /// Gets the monitored host count.
    /// </summary>
    [ObservableProperty]
    private int monitorHostCount;

    /// <summary>
    /// Gets the current user name.
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(
        HostListViewModel hostListViewModel,
        MonitorOrchestrator orchestrator,
        NotificationService notificationService)
    {
        HostListViewModel = hostListViewModel;
        _orchestrator = orchestrator;
        _notificationService = notificationService;
        SnackbarMessageQueue = notificationService.MessageQueue;
        AppVersion = GetAppVersion();
        UserName = Environment.UserName;
        MonitorHostCount = HostListViewModel.Hosts.Count;
        HostListViewModel.Hosts.CollectionChanged += OnHostsCollectionChanged;

        WeakReferenceMessenger.Default.Register<HostChangedMessage>(this, async (_, message) =>
        {
            if (!message.IsEdit && IsMonitoring)
            {
                await StartMonitoringHostAsync(message.Host);
            }
        });
    }

    /// <summary>
    /// Gets the snackbar message queue.
    /// </summary>
    public ISnackbarMessageQueue SnackbarMessageQueue { get; }

    private void OnHostsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MonitorHostCount = HostListViewModel.Hosts.Count;
    }

    private static string GetAppVersion()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        return version is null ? "1.0.0.0" : version.ToString();
    }

    [RelayCommand]
    private async Task StartMonitoringAsync()
    {
        if (IsMonitoring)
        {
            return;
        }

        IsMonitoring = true;
        _monitoringCts?.Cancel();
        _monitoringCts?.Dispose();
        _monitoringCts = new CancellationTokenSource();

        _orchestrator.MonitorResultReceived -= OnMonitorResultReceived;
        _orchestrator.MonitorResultReceived += OnMonitorResultReceived;

        try
        {
            foreach (var host in HostListViewModel.Hosts)
            {
                host.CurrentStatus = HostStatus.Checking;
                host.ResponseTimeHistory.Clear();
                host.ResponseTimeHistory.Add(0);
                _latestResults[host.Id] = new Dictionary<MonitorMethodKey, MonitorResult>();
                await _orchestrator.StartMonitoringAsync(host, _monitoringCts.Token);
            }

            _notificationService.ShowSuccess("已開始監控");
            StartOfflineNotificationTimer();
        }
        catch
        {
            IsMonitoring = false;
            throw;
        }
    }

    private async Task StartMonitoringHostAsync(Host host)
    {
        if (_monitoringCts is null)
        {
            return;
        }

        host.CurrentStatus = HostStatus.Checking;
        host.ResponseTimeHistory.Clear();
        host.ResponseTimeHistory.Add(0);
        _latestResults[host.Id] = new Dictionary<MonitorMethodKey, MonitorResult>();
        await _orchestrator.StartMonitoringAsync(host, _monitoringCts.Token);
    }

    [RelayCommand]
    private void StopMonitoring()
    {
        if (!IsMonitoring)
        {
            return;
        }

        _monitoringCts?.Cancel();
        _monitoringCts?.Dispose();
        _monitoringCts = null;

        StopOfflineNotificationTimer();

        foreach (var host in HostListViewModel.Hosts)
        {
            _orchestrator.StopMonitoring(host.Id);
        }

        _orchestrator.MonitorResultReceived -= OnMonitorResultReceived;
        IsMonitoring = false;
        _notificationService.ShowWarning("已停止監控");
    }

    private void OnMonitorResultReceived(object? sender, MonitorResult result)
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            ApplyResult(result);
            return;
        }

        dispatcher.Invoke(() => ApplyResult(result));
    }

    private void ApplyResult(MonitorResult result)
    {
        var host = HostListViewModel.Hosts.FirstOrDefault(h => h.Id == result.HostId);
        if (host is null)
        {
            return;
        }

        if (!_latestResults.TryGetValue(host.Id, out var hostResults))
        {
            hostResults = new Dictionary<MonitorMethodKey, MonitorResult>();
            _latestResults[host.Id] = hostResults;
        }

        hostResults[new MonitorMethodKey(result.MonitorType, result.Port)] = result;
        host.LastCheckTime = result.CheckTime;

        var expectedKeys = host.MonitorMethods
            .Where(method => method.IsEnabled)
            .Select(method => new MonitorMethodKey(method.Type, method.Port))
            .ToList();

        if (expectedKeys.Count == 0)
        {
            host.CurrentStatus = HostStatus.Unknown;
            host.AverageResponseTimeMs = null;
            host.LastErrorMessage = null;
            return;
        }

        if (expectedKeys.Any(key => !hostResults.ContainsKey(key)))
        {
            host.CurrentStatus = HostStatus.Checking;
            host.AverageResponseTimeMs = null;
            return;
        }

        var previousStatus = host.CurrentStatus;
        var results = expectedKeys.Select(key => hostResults[key]).ToList();
        var successCount = results.Count(r => r.IsSuccess);
        var failureCount = results.Count - successCount;

        var newStatus = successCount == results.Count
            ? HostStatus.Online
            : failureCount == results.Count
                ? HostStatus.Offline
                : HostStatus.Warning;

        host.CurrentStatus = newStatus;
        host.AverageResponseTimeMs = results.Average(r => r.ResponseTimeMs);

        var errors = results
            .Where(r => !string.IsNullOrWhiteSpace(r.ErrorMessage))
            .Select(r => r.ErrorMessage)
            .Distinct();
        host.LastErrorMessage = string.Join("; ", errors);

        if (previousStatus != newStatus)
        {
            if (newStatus == HostStatus.Offline)
            {
                _notificationService.ShowError($"主機離線: {host.Name}");
            }
            else if (newStatus == HostStatus.Online && previousStatus == HostStatus.Offline)
            {
                _notificationService.ShowSuccess($"主機上線: {host.Name}");
            }
        }
    }

    private readonly record struct MonitorMethodKey(MonitorType Type, int? Port);

    private void StartOfflineNotificationTimer()
    {
        StopOfflineNotificationTimer();
        _offlineNotificationTimer = new System.Threading.Timer(
            OnOfflineNotificationTimerTick,
            null,
            TimeSpan.FromSeconds(OfflineNotificationIntervalSeconds),
            TimeSpan.FromSeconds(OfflineNotificationIntervalSeconds));
    }

    private void StopOfflineNotificationTimer()
    {
        _offlineNotificationTimer?.Dispose();
        _offlineNotificationTimer = null;
    }

    private void OnOfflineNotificationTimerTick(object? state)
    {
        if (!IsMonitoring)
        {
            return;
        }

        var offlineHosts = HostListViewModel.Hosts
            .Where(h => h.CurrentStatus == HostStatus.Offline)
            .ToList();

        if (offlineHosts.Count == 0)
        {
            return;
        }

        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            NotifyOfflineHosts(offlineHosts);
            return;
        }

        dispatcher.BeginInvoke(() => NotifyOfflineHosts(offlineHosts));
    }

    private void NotifyOfflineHosts(List<Host> offlineHosts)
    {
        var names = string.Join(", ", offlineHosts.Select(h => h.Name));
        var message = offlineHosts.Count == 1
            ? $"主機離線: {names}"
            : $"{offlineHosts.Count} 台主機離線: {names}";
        _notificationService.ShowError(message);
    }
}
