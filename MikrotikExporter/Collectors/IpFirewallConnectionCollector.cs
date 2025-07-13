using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// Ip Firewall Connection collector
///
/// Frequency of change: High. Personally I plan to collect it every second
/// </summary>
/// <remarks>
/// In theory, which I am going to test, you can track how much internet is used by each host but only if you collect if frequently, like every second.
/// But you must include only IPs addresses that are part of the internet which is indicated by src_is_internet and dst_is_internet.
///
/// I guess in case of NAT, which is case for I guess almost all applications in IPv4, SRC is always private address.
/// At least in my case.
///
/// TODO: I could add here DNS Cache entries to lookup site's IP Address and include that too in order to create
/// unlimited 1984 monitoring power! Although with great power comes great responsibility...
/// 
/// </remarks>
public class IpFirewallConnectionCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public IpFirewallConnectionCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true, CancellationToken cancellationToken = default)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var ipFirewallConnectionsTask = _client.GetIpFirewallConnections(cancellationToken);
        var dhcpServerLeasesTask = _client.GetDhcpServerLeases(cancellationToken);
        var dnsCacheRecordsTask = _client.GetDnsCacheRecords(cancellationToken);
        
        await Task.WhenAll(ipFirewallConnectionsTask,  dhcpServerLeasesTask, dnsCacheRecordsTask);
        
        var ipFirewallConnections = ipFirewallConnectionsTask.Result;
        var dhcpServerLeases = dhcpServerLeasesTask.Result;
        var dnsCacheRecords = dnsCacheRecordsTask.Result;
        
        return Map(ipFirewallConnections, dhcpServerLeases, dnsCacheRecords);
    }
    
    private MetricsCollection<IpFirewallConnection> Map(
        IpFirewallConnection[] connections,
        DhcpServerLease[] dhcpServerLeases,
        DnsCacheRecord[] dnsCacheRecords)
    {

        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["host"] = _client.Host,
        };

        MetricsCollection<IpFirewallConnection> collection = new();
        collection.CreateValueSets(
            staticLabels,
            Timeout,
            OrigPackets, OrigBytes, ReplPackets, ReplBytes,
            OrigFasttrackPackets, OrigFasttrackBytes,
            ReplFasttrackPackets, ReplFasttrackBytes,
            OrigRate, ReplRate
        );
        
        foreach (var connection in connections)
        {
            Dictionary<string, string> labels = new()
            {
                ["id"] = connection.Id
            };
            
            labels.AddIfNotEmpty("protocol", connection.Protocol);
            labels.AddIfNotEmpty("reply_src_address", connection.ReplySrcAddress);
            labels.AddIfNotEmpty("reply_dst_address", connection.ReplyDstAddress);
            labels.AddIfNotEmpty("expected", connection.Expected);
            labels.AddIfNotEmpty("seen_reply", connection.SeenReply);
            labels.AddIfNotEmpty("assured", connection.Assured);
            labels.AddIfNotEmpty("fasttrack", connection.Fasttrack);
            labels.AddIfNotEmpty("hw_offload", connection.HwOffload);
            labels.AddIfNotEmpty("src_nat", connection.SrcNat);
            labels.AddIfNotEmpty("dst_nat", connection.DstNat);
            labels.AddIfNotEmpty("confirmed", connection.Confirmed);
            labels.AddIfNotEmpty("dying", connection.Dying);


            if (!string.IsNullOrEmpty(connection.SrcAddress))
            {
                var splitted = connection.SrcAddress.Split(':', StringSplitOptions.RemoveEmptyEntries);
                labels.Add("src_address", splitted[0]);

                if (splitted.Length > 1)
                {
                    labels.Add("src_port", splitted[1]);
                }
                
                // Try to find hostname for address
                var srcHostname = dhcpServerLeases
                    .FirstOrDefault(l => l.Address == splitted[0])
                    ?.HostName ?? string.Empty;
                
                labels.AddIfNotEmpty("src_hostname", srcHostname);
                
                // Try to find domain
                var srcDomain = dnsCacheRecords
                    .FirstOrDefault(d => d.Type == "A" && d.Data == splitted[0])
                    ?.Name ?? string.Empty;
                
                labels.AddIfNotEmpty("src_domain", srcDomain);
                
                labels.Add("src_is_public", IpHelpers.IsPublic(splitted[0]).ToString());
            }

            if (!string.IsNullOrEmpty(connection.DstAddress))
            {
                var splitted =  connection.DstAddress.Split(':', StringSplitOptions.RemoveEmptyEntries);
                
                labels.Add("dst_address", splitted[0]);

                if (splitted.Length > 1)
                {
                    labels.Add("dst_port", splitted[1]);
                }
                
                var dstHostname = dhcpServerLeases
                    .FirstOrDefault(l => l.Address == splitted[0])
                    ?.HostName ?? string.Empty;
            
                labels.AddIfNotEmpty("dst_hostname", dstHostname);
                
                var dstDomain = dnsCacheRecords
                    .FirstOrDefault(d => d.Type == "A" && d.Data == splitted[0])
                    ?.Name ?? string.Empty;
                
                labels.AddIfNotEmpty("dst_domain", dstDomain);
                labels.Add("dst_is_public", IpHelpers.IsPublic(splitted[0]).ToString());
            }

            collection.AddValue(connection, labels);
        }

        return collection;
    }
 private static readonly Gauge<IpFirewallConnection> Timeout = new(
        "mikrotik_firewall_connection_timeout",
        "Timeout of firewall connection, i.e. how much times left before connection will face cruel death",
        i => string.IsNullOrEmpty(i.Timeout)
            ? null
            : MikrotikTimeSpanConverter.ToSeconds(i.Timeout).ToString(CultureInfo.InvariantCulture)
    );
    
    private static readonly Counter<IpFirewallConnection> OrigPackets = new(
        "mikrotik_firewall_connection_orig_packets_total",
        "Originating packets from source to destination within connection. In other words, it's sent packets",
        i => i.OrigPackets
    );

    private static readonly Counter<IpFirewallConnection> OrigBytes = new(
        "mikrotik_firewall_connection_orig_bytes_total",
        "Originating bytes from source to destination within connection. In other words, it's sent bytes",
        i => i.OrigBytes
    );

    private static readonly Counter<IpFirewallConnection> ReplPackets = new(
        "mikrotik_firewall_connection_repl_packets_total",
        "Reply packets from destination to source within connection. In other words, it's received packets",
        i => i.ReplPackets
    );

    private static readonly Counter<IpFirewallConnection> ReplBytes = new(
        "mikrotik_firewall_connection_repl_bytes_total",
        "Reply bytes from destination to source within connection. In other words, it's received bytes",
        i => i.ReplBytes
    );

    private static readonly Counter<IpFirewallConnection> OrigFasttrackPackets = new(
        "mikrotik_firewall_connection_orig_fasttrack_packets_total",
        "Originating fast tracked packets.",
        i => i.OrigFasttrackPackets
    );

    private static readonly Counter<IpFirewallConnection> OrigFasttrackBytes = new(
        "mikrotik_firewall_connection_orig_fasttrack_bytes_total",
        "Originating fast tracked bytes.",
        i => i.OrigFasttrackBytes
    );

    private static readonly Counter<IpFirewallConnection> ReplFasttrackPackets = new(
        "mikrotik_firewall_connection_repl_fasttrack_packets_total",
        "Reply fast tracked packets.",
        i => i.ReplFasttrackPackets
    );

    private static readonly Counter<IpFirewallConnection> ReplFasttrackBytes = new(
        "mikrotik_firewall_connection_repl_fasttrack_bytes_total",
        "Reply fast tracked bytes.",
        i => i.ReplFasttrackBytes
    );

    private static readonly Gauge<IpFirewallConnection> OrigRate = new(
        "mikrotik_firewall_connection_orig_rate",
        "Mikrotik firewall connection originating connection rate",
        i => i.OrigRate
    );

    private static readonly Gauge<IpFirewallConnection> ReplRate = new(
        "mikrotik_firewall_connection_repl_rate",
        "Mikrotik firewall connection reply rate",
        i => i.ReplRate
    );
}
