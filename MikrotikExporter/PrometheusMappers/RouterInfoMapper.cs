using System.Globalization;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;

namespace MikrotikExporter.PrometheusMappers;

public static class RouterInfoMapper
{
    public static MetricsCollection[] Map(SystemResource resource, string identity, string routerName, string routerHost)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = routerName,
            ["router_host"] = routerHost,
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

    private static readonly Gauge<SystemResource> Uptime = new(
        "mikrotik_uptime",
        "Uptime",
        i => MikrotikTimeSpanConverter.ToSeconds(i.Uptime).ToString(CultureInfo.InvariantCulture)
    );
}
