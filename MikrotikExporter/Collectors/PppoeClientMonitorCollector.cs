using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// PPPoE Client monitor collector
///
/// Frequency of change: low, not worth collecting more frequently than once per 30 seconds I guess
/// </summary>
public class PppoeClientMonitorCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public PppoeClientMonitorCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }

        var interfaces = await _client.GetInterfaces();
        
        var ifaces =
            interfaces.Where(i => i.Type == "pppoe-out")
                .ToArray();
        
        var monitors = await _client.GetPppoeClientMonitor(ifaces.Select(i => i.Id));

        for (var i = 0; i < Math.Min(monitors.Length, ifaces.Length); i++)
        {
            monitors[i].Name = ifaces[i].Name;
        }

        return Map(monitors);
    }
    
    private MetricsCollection<PppoeClientMonitor> Map(PppoeClientMonitor[] monitors)
    {
        var collection = new MetricsCollection<PppoeClientMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = _client.Name, ["router_host"] = _client.Host, ["type"] = "pppoe-client"
        };

        collection.CreateValueSets(
            routerLabels,
            UptimeSeconds, ActiveLinks, Mtu, Mru
        );

        foreach (var monitor in monitors)
        {
            Dictionary<string, string> labels = new()
            {
                ["name"] = monitor.Name,
                ["status"] = monitor.Status
            };
            
            labels.AddIfNotEmpty("encoding", monitor.Encoding);
            labels.AddIfNotEmpty("service_name", monitor.ServiceName);
            labels.AddIfNotEmpty("ac_name", monitor.AcName);
            labels.AddIfNotEmpty("ac_mac", monitor.AcMac);
            labels.AddIfNotEmpty("local_address", monitor.LocalAddress);
            labels.AddIfNotEmpty("remote_address", monitor.RemoteAddress);
            
            collection.AddValue(monitor, labels);
        }

        return collection;
    }
    
    private static readonly Gauge<PppoeClientMonitor> UptimeSeconds = new(
        "mikrotik_pppoe_client_uptime_seconds",
        "PPPoE client connection uptime in seconds",
        i => MikrotikTimeSpanConverter.ToSeconds(i.Uptime).ToString(CultureInfo.InvariantCulture)
    );

    private static readonly Gauge<PppoeClientMonitor> ActiveLinks = new(
        "mikrotik_pppoe_client_active_links",
        "Number of active PPPoE client links",
        i => i.ActiveLinks
    );

    private static readonly Gauge<PppoeClientMonitor> Mtu = new(
        "mikrotik_pppoe_client_mtu",
        "PPPoE client MTU",
        i => i.Mtu
    );

    private static readonly Gauge<PppoeClientMonitor> Mru = new(
        "mikrotik_pppoe_client_mru",
        "PPPoE client MRU",
        i => i.Mru
    );
}
