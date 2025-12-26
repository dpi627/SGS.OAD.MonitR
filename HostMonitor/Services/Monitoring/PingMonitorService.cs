using System.Net.NetworkInformation;
using HostMonitor.Models;
using HostMonitor.Models.Enums;
using HostMonitor.Services.Interfaces;

namespace HostMonitor.Services.Monitoring;

/// <summary>
/// Performs ICMP ping monitoring.
/// </summary>
public class PingMonitorService : IMonitorService
{
    /// <inheritdoc />
    public MonitorType SupportedType => MonitorType.IcmpPing;

    /// <inheritdoc />
    public async Task<MonitorResult> CheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken = default)
    {
        var result = new MonitorResult
        {
            HostId = host.Id,
            MonitorType = SupportedType,
            CheckTime = DateTime.Now
        };

        try
        {
            using var ping = new Ping();
            var timeout = method.TimeoutMs > 0 ? method.TimeoutMs : 5000;
            var reply = await ping.SendPingAsync(host.HostnameOrIp, timeout);

            result.IsSuccess = reply.Status == IPStatus.Success;
            result.ResponseTimeMs = reply.RoundtripTime;
            result.ErrorMessage = result.IsSuccess ? null : reply.Status.ToString();
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}
