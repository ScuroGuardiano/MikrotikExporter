using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// Frequency of change: High if you care about RX/TX rates, otherwise low. For me it's low.
/// </summary>
public class IpFirewallRuleCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public IpFirewallRuleCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true, CancellationToken cancellationToken = default)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var rules = await _client.GetIpFirewallRules(cancellationToken);
        
        return Map(rules);
    }
    
    private MetricsCollection<IpFirewallRule> Map(IpFirewallRule[] rules)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host
        };

        MetricsCollection<IpFirewallRule> collection = new();
        collection.CreateValueSets(
            staticLabels,
            FwBytes, FwPackets
        );
        
        foreach (var rule in rules)
        {
            Dictionary<string, string> labels = new()
            {
                ["id"] = rule.Id,
                ["chain"] = rule.Chain,
                ["action"] = rule.Action,
                ["table"] = rule.Table
            };
            
            labels.AddIfNotEmpty("comment", rule.Comment);
            labels.AddIfNotEmpty("connection_state",  rule.ConnectionState);
            labels.AddIfNotEmpty("connection_nat_state", rule.ConnectionNatState);
            labels.AddIfNotEmpty("in_interface", rule.InInterface);
            labels.AddIfNotEmpty("out_interface", rule.OutInterface);
            labels.AddIfNotEmpty("in_interface_list",  rule.InInterfaceList);
            labels.AddIfNotEmpty("out_interface_list", rule.OutInterfaceList);
            labels.AddIfNotEmpty("src_port", rule.SrcPort);
            labels.AddIfNotEmpty("dst_port", rule.DstPort);
            labels.AddIfNotEmpty("src_address", rule.SrcAddress);
            labels.AddIfNotEmpty("dst_address", rule.DstAddress);
            labels.AddIfNotEmpty("src_address_list", rule.SrcAddressList);
            labels.AddIfNotEmpty("dst_address_list", rule.DstAddressList);
            labels.AddIfNotEmpty("to_addresses", rule.ToAddresses);
            labels.AddIfNotEmpty("to_ports", rule.ToPorts);
            labels.AddIfNotEmpty("invalid", rule.Invalid);
            labels.AddIfNotEmpty("dynamic", rule.Dynamic);
            labels.AddIfNotEmpty("disabled", rule.Disabled);
            
            collection.AddValue(rule, labels);
        }
        
        return collection;
    }
    
    private static readonly Counter<IpFirewallRule> FwBytes = new (
        "mikrotik_ip_firewall_rule_bytes",
        "Passed bytes through the firewall rule",
        i => i.Bytes
    );

    private static readonly Counter<IpFirewallRule> FwPackets = new(
        "mikrotik_ip_firewall_rule_packets",
        "Passed packets through the firewall rule",
        i => i.Packets
    );
}
