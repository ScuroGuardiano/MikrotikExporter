using MikrotikApiClient.Dto;

namespace MikrotikExporter.PrometheusMappers;

public static class InterfaceSummaryMapper
{
    public static MetricsCollection Map(InterfaceSummary[] interfaces, string routerName, string routerHost)
    {
        var collection = new MetricsCollection<InterfaceSummary>();

        var routerLabels = new Dictionary<string, string> { ["router"] = routerName, ["router_host"] = routerHost };

        collection.CreateValueSets(
            routerLabels,
            RxBytes, TxBytes, RxPackets, TxPackets,
            FpRxBytes,  FpTxBytes, FpRxPackets, FpTxPackets,
            RxDrop, TxDrop, TxQueueDrop, RxError, TxError,
            Running, LinksDown, LastLinkUpTime, LastLinkDownTime
        );

        foreach (var inter in interfaces)
        {
            Dictionary<string, string> labels = new()
            {
                ["name"] = inter.Name,
            };
            
            labels.AddIfNotEmpty("mac_address", inter.MacAddress);
            labels.AddIfNotEmpty("type", inter.Type);
            labels.AddIfNotEmpty("comment", inter.Comment);
            
            collection.AddValue(inter, labels);
        }

        return collection;
    }

    private static readonly Counter<InterfaceSummary> RxBytes = new (
        "mikrotik_rx_bytes",
        "Bytes received",
        i => i.RxByte
    );

    private static readonly Counter<InterfaceSummary> TxBytes = new (
        "mikrotik_tx_bytes",
        "Bytes sent",
        i => i.TxByte
    );

    private static readonly Counter<InterfaceSummary> RxPackets = new (
        "mikrotik_rx_packets",
        "Packets received",
        i => i.RxPacket
    );

    private static readonly Counter<InterfaceSummary> TxPackets = new (
        "mikrotik_tx_packets",
        "Packets sent",
        i => i.TxPacket
    );

    private static readonly Counter<InterfaceSummary> FpRxBytes = new (
        "mikrotik_fp_rx_bytes",
        "Fast Path bytes received",
        i => i.FpRxByte
    );
    
    private static readonly Counter<InterfaceSummary> FpTxBytes = new (
        "mikrotik_fp_tx_bytes",
        "Fast Path bytes sent",
        i => i.FpTxByte
    );
    
    private static readonly Counter<InterfaceSummary> FpRxPackets = new (
        "mikrotik_fp_rx_packets",
        "Fast Path packets received",
        i => i.FpRxPacket
    );
    
    private static readonly Counter<InterfaceSummary> FpTxPackets = new (
        "mikrotik_fp_tx_packets",
        "Fast Path packets sent",
        i => i.FpTxPacket
    );
    
    private static readonly Counter<InterfaceSummary> RxDrop = new (
        "mikrotik_rx_drop",
        "RX dropped",
        i => i.RxDrop
    );
    
    private static readonly Counter<InterfaceSummary> TxDrop = new (
        "mikrotik_tx_drop",
        "TX dropped",
        i => i.TxDrop
    );
    
    private static readonly Counter<InterfaceSummary> TxQueueDrop = new (
        "mikrotik_tx_queue_drop",
        "TX queue dropped",
        i => i.TxQueueDrop
    );
    
    private static readonly Counter<InterfaceSummary> RxError = new (
        "mikrotik_rx_error",
        "RX error",
        i => i.RxError
    );
    
    private static readonly Counter<InterfaceSummary> TxError = new (
        "mikrotik_tx_error",
        "TX error",
        i => i.TxError
    );
    
    private static readonly Gauge<InterfaceSummary> Running = new (
        "mikrotik_interface_running",
        "Is interface running",
        i => i.Running == "true" ? "1" : "0"
    );
    
    private static readonly Counter<InterfaceSummary> LinksDown = new (
        "mikrotik_links_downs",
        "Links down",
        i => i.LinkDowns
    );
    
    private static readonly Gauge<InterfaceSummary> LastLinkDownTime = new (
        "mikrotik_last_link_down_time",
        "Last link down time",
        i =>
        {
            if (string.IsNullOrEmpty(i.LastLinkDownTime))
            {
                return null;
            }

            var x = i.LastLinkDownTime.Replace(' ', 'T');
            if (DateTime.TryParse(x, out DateTime lastLinkDownTime))
            {
                // Router returns time in local timestamp, so... yeah...
                // Basically this exporter and router must be in the same timezone
                DateTimeOffset xd = DateTime.SpecifyKind(lastLinkDownTime, DateTimeKind.Local);

                return xd.ToUnixTimeMilliseconds().ToString();
            }
            
            return null;
        }
    );
    
    private static readonly Gauge<InterfaceSummary> LastLinkUpTime = new (
        "mikrotik_last_link_up_time",
        "Last link up time",
        i =>
        {
            if (string.IsNullOrEmpty(i.LastLinkUpTime))
            {
                return null;
            }

            var x = i.LastLinkUpTime.Replace(' ', 'T');
            if (DateTime.TryParse(x, out DateTime lastLinkUpTime))
            {
                // Router returns time in local timestamp, so... yeah...
                // Basically this exporter and router must be in the same timezone
                DateTimeOffset xd = DateTime.SpecifyKind(lastLinkUpTime, DateTimeKind.Local);

                return xd.ToUnixTimeMilliseconds().ToString();
            }
            
            return null;
        }
    );
}
