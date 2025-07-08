using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

public class IpPoolCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public IpPoolCollector(IMikrotikApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect()
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var ipPools = await _client.GetIpPools();

        return Map(ipPools);
    }
    
    private MetricsCollection<IpPool> Map(IpPool[] ipPools)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] =  _client.Host
        };
        
        MetricsCollection<IpPool> collection = new();
        collection.CreateValueSets(
            staticLabels,
            Used, Total, Available
        );

        foreach (var ipPool in ipPools)
        {
            Dictionary<string, string> labels = new()
            {
                ["id"] = ipPool.Id,
                ["name"] = ipPool.Name,
                ["ranges"]  = ipPool.Ranges,
            };
            
            labels.AddIfNotEmpty("comment", ipPool.Comment);
            
            collection.AddValue(ipPool, labels);
        }
        
        return collection;
    }

    private static readonly Gauge<IpPool> Used = new(
        "mikrotik_ip_pool_used",
        "Used addresses of a given IP pool",
        i => i.Used
    );

    private static readonly Gauge<IpPool> Total = new(
        "mikrotik_ip_pool_total",
        "Total addresses of a given IP pool",
        i => i.Total
    );

    private static readonly Gauge<IpPool> Available = new(
        "mikrotik_ip_pool_available",
        "Available addresses left of a given IP pool",
        i => i.Available
    );
}