using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.Services.Monitoring;

/// <summary>
/// Coordinates multiple monitoring services.
/// </summary>
public class MonitorOrchestrator
{
    private readonly Dictionary<MonitorType, IMonitorService> _serviceMap;
    private readonly Dictionary<Guid, CancellationTokenSource> _monitoringTasks;
    private readonly Dictionary<Guid, List<Task>> _activeTasks;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitorOrchestrator"/> class.
    /// </summary>
    public MonitorOrchestrator(IEnumerable<IMonitorService> monitorServices)
    {
        _serviceMap = monitorServices.ToDictionary(service => service.SupportedType);
        _monitoringTasks = new Dictionary<Guid, CancellationTokenSource>();
        _activeTasks = new Dictionary<Guid, List<Task>>();
    }

    /// <summary>
    /// Gets the number of registered monitor services.
    /// </summary>
    public int ServiceCount => _serviceMap.Count;

    /// <summary>
    /// Raised when a monitor result is received.
    /// </summary>
    public event EventHandler<MonitorResult>? MonitorResultReceived;

    /// <summary>
    /// Raised when a monitoring command is issued.
    /// </summary>
    public event EventHandler<MonitorCommandEventArgs>? MonitorCommandIssued;

    /// <summary>
    /// Starts monitoring a host.
    /// </summary>
    public Task StartMonitoringAsync(Host host, CancellationToken cancellationToken = default)
    {
        StopMonitoring(host.Id);

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _monitoringTasks[host.Id] = linkedCts;

        var tasks = new List<Task>();
        foreach (var method in host.MonitorMethods.Where(m => m.IsEnabled))
        {
            tasks.Add(RunMonitorLoopAsync(host, method, linkedCts.Token));
        }

        _activeTasks[host.Id] = tasks;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops monitoring a host.
    /// </summary>
    public void StopMonitoring(Guid hostId)
    {
        if (_monitoringTasks.TryGetValue(hostId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _monitoringTasks.Remove(hostId);
        }

        if (_activeTasks.ContainsKey(hostId))
        {
            _activeTasks.Remove(hostId);
        }
    }

    /// <summary>
    /// Executes a one-time check for a host.
    /// </summary>
    public async Task<List<MonitorResult>> CheckHostAsync(Host host, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<MonitorResult>>();
        foreach (var method in host.MonitorMethods.Where(m => m.IsEnabled))
        {
            tasks.Add(ExecuteCheckAsync(host, method, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    private async Task RunMonitorLoopAsync(Host host, MonitorMethod method, CancellationToken cancellationToken)
    {
        var intervalSeconds = method.IntervalSeconds > 0 ? method.IntervalSeconds : 60;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));

        try
        {
            var initialResult = await ExecuteCheckAsync(host, method, cancellationToken);
            MonitorResultReceived?.Invoke(this, initialResult);

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var result = await ExecuteCheckAsync(host, method, cancellationToken);
                MonitorResultReceived?.Invoke(this, result);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task<MonitorResult> ExecuteCheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken)
    {
        var command = BuildCommandText(host, method);
        MonitorCommandIssued?.Invoke(this, new MonitorCommandEventArgs(host.Id, command, DateTime.Now));

        var service = GetMonitorService(method.Type);
        return await service.CheckAsync(host, method, cancellationToken);
    }

    private static string BuildCommandText(Host host, MonitorMethod method)
    {
        var target = host.HostnameOrIp;
        return method.Type switch
        {
            MonitorType.IcmpPing => $"PING {target} timeout={method.TimeoutMs}ms",
            MonitorType.TcpPort => $"TCP {target}:{method.Port} timeout={method.TimeoutMs}ms",
            _ => $"{method.Type} {target}"
        };
    }

    private IMonitorService GetMonitorService(MonitorType type)
    {
        if (_serviceMap.TryGetValue(type, out var service))
        {
            return service;
        }

        throw new InvalidOperationException($"No monitor service registered for {type}.");
    }
}
