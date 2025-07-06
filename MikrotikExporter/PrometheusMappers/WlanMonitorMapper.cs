using MikrotikApiClient.Dto;

namespace MikrotikExporter.PrometheusMappers;

public static class WlanMonitorMapper
{
    public static MetricsCollection Map(WlanMonitor[] monitors, string routerName, string routerHost)
    {
        var collection = new MetricsCollection<WlanMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = routerName, ["router_host"] = routerHost, ["type"] = "wlan"
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