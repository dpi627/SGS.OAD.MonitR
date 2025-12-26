using System.Diagnostics;
using System.Net.Sockets;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.Services.Monitoring;

/// <summary>
/// Performs TCP port monitoring.
/// </summary>
public class TcpPortMonitorService : IMonitorService
{
    /// <inheritdoc />
    public MonitorType SupportedType => MonitorType.TcpPort;

    /// <inheritdoc />
    public async Task<MonitorResult> CheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken = default)
    {
        if (!method.Port.HasValue)
        {
            throw new ArgumentException("TCP port is required for TcpPort monitoring.", nameof(method));
        }

        var result = new MonitorResult
        {
            HostId = host.Id,
            MonitorType = SupportedType,
            CheckTime = DateTime.Now,
            Port = method.Port
        };

        using var client = new TcpClient();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var timeout = method.TimeoutMs > 0 ? method.TimeoutMs : 5000;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            await client.ConnectAsync(host.HostnameOrIp, method.Port.Value, cts.Token);

            stopwatch.Stop();
            result.IsSuccess = true;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
        }
        catch (Exception ex) when (ex is SocketException or TaskCanceledException or OperationCanceledException)
        {
            stopwatch.Stop();
            result.IsSuccess = false;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.IsSuccess = false;
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}
