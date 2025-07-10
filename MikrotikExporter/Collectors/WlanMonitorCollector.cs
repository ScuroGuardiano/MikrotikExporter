using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;


/// <summary>
/// Wireless Monitor Collector
///
/// Frequency of change: medium, not worth collecting more frequently than once every 10 seconds
/// </summary>
public class WlanMonitorCollector : BaseCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public WlanMonitorCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect()
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var interfaces = await _client.GetInterfaces();
        
        var ifaces = interfaces.Where(i => i.Type == "wlan").ToArray();
        
        var monitors = await _client.GetWlanMonitor(ifaces.Select(i => i.Id));
        
        for (var i = 0; i < Math.Min(monitors.Length, ifaces.Length); i++)
        {
            monitors[i].Name = ifaces[i].Name;
        }

        return Map(monitors);
    }
    
    public MetricsCollection<WlanMonitor> Map(WlanMonitor[] monitors)
    {
        var collection = new MetricsCollection<WlanMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = _client.Name, ["router_host"] = _client.Host, ["type"] = "wlan"
        };

        collection.CreateValueSets(
            routerLabels,
            NoiseFloor, OverallTxCcq, RegisteredClients, AuthenticatedClients,
            WmmEnabled, NotifyExternalFdb
        );

        foreach (var monitor in monitors)
        {
            Dictionary<string, string> labels = new()
            {
                ["name"] = monitor.Name,
                ["status"] = monitor.Status
            };
            
            labels.AddIfNotEmpty("channel", monitor.Channel);
            labels.AddIfNotEmpty("wireless_protocol", monitor.WirelessProtocol);
            
            collection.AddValue(monitor, labels);
        }

        return collection;
    }
    
    
    private static readonly Gauge<WlanMonitor> NoiseFloor = new(
        "mikrotik_wlan_noise_floor",
        "WLAN noise floor measurement",
        i => i.NoiseFloor
    );

    private static readonly Gauge<WlanMonitor> OverallTxCcq = new(
        "mikrotik_wlan_overall_tx_ccq",
        "Overall transmission CCQ (Client Connection Quality)",
        i => i.OverallTxCcq
    );

    private static readonly Gauge<WlanMonitor> RegisteredClients = new(
        "mikrotik_wlan_registered_clients",
        "Number of registered clients",
        i => i.RegisteredClients
    );

    private static readonly Gauge<WlanMonitor> AuthenticatedClients = new(
        "mikrotik_wlan_authenticated_clients",
        "Number of authenticated clients",
        i => i.AuthenticatedClients
    );

    private static readonly Gauge<WlanMonitor> WmmEnabled = new(
        "mikrotik_wlan_wmm_enabled",
        "Whether WMM (WiFi Multimedia) is enabled",
        i => i.WmmEnabled == "true" ? "1" : "0"
    );

    private static readonly Gauge<WlanMonitor> NotifyExternalFdb = new(
        "mikrotik_wlan_notify_external_fdb",
        "External FDB notification setting",
        i => i.NotifyExternalFdb == "true" ? "1" : "0"
    );
}
