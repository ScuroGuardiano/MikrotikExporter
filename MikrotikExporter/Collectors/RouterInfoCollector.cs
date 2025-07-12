using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// Router info collector
///
/// Frequency of change: none
/// </summary>
public class RouterInfoCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public RouterInfoCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var systemResourceTask = _client.GetSystemResource();
        var identityTask = _client.GetIdentity();
        
        await Task.WhenAll(systemResourceTask, identityTask);
        
        var systemResource = systemResourceTask.Result;
        var identity = identityTask.Result;
        
        return Map(systemResource, identity);
    }
    
    private MetricsCollection Map(SystemResource resource, string identity)
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
        var identityCollection = new MetricsCollection<SystemResource>();
        
        systemResourcesCollection.CreateValueSets(staticLabels,
            TotalMemory, CpuCount, TotalHddSpace
        );
        
        systemResourcesCollection.AddValue(resource);
        
        identityCollection.CreateValueSets(staticLabels, RouterInfo);
        
        // Type is only to match type with system resource collector xD
        // My library is shiiiiiiiit
        identityCollection.AddValue(resource, routerInfoLabels);
        
        return MetricsCollection<SystemResource>.Merge([systemResourcesCollection, identityCollection]);
    }
    
    private static readonly Gauge<SystemResource> RouterInfo = new(
        "mikrotik_router_info",
        "Router info",
        i => "1.0"
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
}