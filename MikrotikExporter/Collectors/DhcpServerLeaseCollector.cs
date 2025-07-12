using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikApiClient.Tcp;
using MikrotikExporter.Helpers;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// DHCP Server lease collector
///
/// Frequency of change: medium? Like idk, I'd collect it once every 30 seconds tbh
/// </summary>
public class DhcpServerLeaseCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public DhcpServerLeaseCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var dhcpLeases = await _client.GetDhcpServerLeases();

        return Map(dhcpLeases);
    }

    private MetricsCollection<DhcpServerLease> Map(DhcpServerLease[] leases)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host
        };

        MetricsCollection<DhcpServerLease> collection = new();
        collection.CreateValueSets(
            staticLabels,
            ExpiresAfter, LastSeen, Age
        );
        
        foreach (var lease in leases)
        {
            Dictionary<string, string> labels = new()
            {
                ["id"] = lease.Id,
                ["server"] = lease.Server,
                ["status"] = lease.Status,
                ["mac_address"] = lease.MacAddress,
                ["client_id"] = lease.ClientId,
                ["radius"] = lease.Radius,
                ["dynamic"] = lease.Dynamic,
                ["blocked"] = lease.Blocked,
                ["disabled"] =  lease.Disabled
            };
            
            labels.AddIfNotEmpty("host_name", lease.HostName);
            labels.AddIfNotEmpty("comment", lease.Comment);
            labels.AddIfNotEmpty("active_address", lease.ActiveAddress);
            labels.AddIfNotEmpty("active_mac_address", lease.ActiveMacAddress);
            labels.AddIfNotEmpty("active_client_id", lease.ActiveClientId);
            labels.AddIfNotEmpty("active_server", lease.ActiveServer);
            labels.AddIfNotEmpty("class_id", lease.ClassId);
            
            collection.AddValue(lease, labels);
        }
        
        return collection;
    }

    private static readonly Gauge<DhcpServerLease> ExpiresAfter = new(
        "mikrotik_dhcp_server_lease_expires_after",
        "Time to expiration of DHCP Lease",
        i => string.IsNullOrEmpty(i.ExpiresAfter)
            ? null
            : MikrotikTimeSpanConverter.ToSeconds(i.ExpiresAfter).ToString(CultureInfo.InvariantCulture)
    );

    private static readonly Gauge<DhcpServerLease> Age = new(
        "mikrotik_dhcp_server_lease_age",
        "Age of DHCP Lease",
        i => string.IsNullOrEmpty(i.Age) 
            ? null
            : MikrotikTimeSpanConverter.ToSeconds(i.Age).ToString(CultureInfo.InvariantCulture)
    );

    private static readonly Gauge<DhcpServerLease> LastSeen = new(
        "mikrotik_dhcp_server_lease_last_seen",
        "How many time passed since host has been last seen",
        i => string.IsNullOrEmpty(i.LastSeen)
            ? null
            : MikrotikTimeSpanConverter.ToSeconds(i.LastSeen).ToString(CultureInfo.InvariantCulture)
    );

}