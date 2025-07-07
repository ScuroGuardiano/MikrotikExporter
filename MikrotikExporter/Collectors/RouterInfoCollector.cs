using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// Router info collector
///
/// Frequency of change: none, except for Uptime.
/// </summary>
public class RouterInfoCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public RouterInfoCollector(IMikrotikApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection[]> Collect()
    {
        if (!Enabled)
        {
            return [];
        }
        
        var systemResource = await _client.GetSystemResource();
        var identity = await _client.GetIdentity();
        
        return Map(systemResource, identity);
    }
    
    private MetricsCollection[] Map(SystemResource resource, string identity)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host,
        };

        Dictionary<string, string> routerInfoLabels = new()
        {
            ["router_identity"] = identity,
            ["version"] = resource.Version,
            ["factory_software"] = resource.FactorySoftware,
            ["build_time"] = resource.BuildTime,
            ["cpu"] = resource.Cpu,
            ["architecture_name"] = resource.ArchitectureName,
            ["board_name"] = resource.BoardName,
            ["platform"] = resource.Platform
        };
        
        var systemResourcesCollection = new MetricsCollection<SystemResource>();
        var identityCollection = new MetricsCollection<string>();
        
        systemResourcesCollection.CreateValueSets(staticLabels,
            TotalMemory, CpuCount, TotalHddSpace, Uptime
        );
        
        systemResourcesCollection.AddValue(resource);
        
        identityCollection.CreateValueSets(staticLabels, RouterInfo);
        identityCollection.AddValue(identity, routerInfoLabels);
        
        return [systemResourcesCollection, identityCollection];
    }
    
    private static readonly Gauge<string> RouterInfo = new(
        "mikrotik_router_info",
        "Router info",
        i => i
    );

    private static readonly Gauge<SystemResource> TotalMemory = new(
        "mikrotik_total_memory",
        "Total RAM memory",
        i => i.TotalMemory
    );

    private static readonly Gauge<SystemResource> CpuCount = new(
        "mikrotik_cpu_count",
        "Total CPU count",
        i => i.CpuCount
    );

    private static readonly Gauge<SystemResource> TotalHddSpace = new(
        "mikrotik_total_hdd_space",
        "Total HDD space",
        i => i.TotalHddSpace
    );

    private static readonly Gauge<SystemResource> Uptime = new(
        "mikrotik_uptime",
        "Uptime",
        i => MikrotikTimeSpanConverter.ToSeconds(i.Uptime).ToString(CultureInfo.InvariantCulture)
    );
}