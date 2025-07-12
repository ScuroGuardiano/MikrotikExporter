using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// This collector collects only those resource that change over time, like currently used RAM or CPU
/// Other stats, like total RAM, total HDD space etc. you will get with <see cref="RouterInfoCollector"/>
/// </summary>
public class SystemResourceCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public SystemResourceCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var resource = await _client.GetSystemResource();

        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host
        };
        
        var collection = new MetricsCollection<SystemResource>();
        collection.CreateValueSets(
            staticLabels,
            Uptime, CpuFrequency, CpuLoad,
            FreeMemory, FreeHddSpace,
            WrittenSectSinceReboot, WrittenSectorsTotal,
            BadBlocks
        );
        
        collection.AddValue(resource);
        
        return collection;
    }
    
    
    private static readonly Gauge<SystemResource> Uptime = new(
        "mikrotik_uptime",
        "",
        i => MikrotikTimeSpanConverter.ToSeconds(i.Uptime).ToString(CultureInfo.InvariantCulture)
    );

    private static readonly Gauge<SystemResource> CpuFrequency = new(
        "mikrotik_cpu_frequency",
        "",
        i => i.CpuFrequency
    );

    private static readonly Gauge<SystemResource> CpuLoad = new(
        "mikrotik_cpu_load",
        "",
        i => i.CpuLoad
    );

    private static readonly Gauge<SystemResource> FreeMemory = new(
        "mikrotik_free_memory",
        "",
        i => i.FreeMemory
    );
    
    private static readonly Gauge<SystemResource> FreeHddSpace = new(
        "mikrotik_free_hdd_space",
        "",
        i => i.FreeHddSpace
    );

    private static readonly Gauge<SystemResource> WrittenSectSinceReboot = new(
        "mikrotik_written_sectors_since_reboot",
        "Written sectors to HDD since reboot",
        i => i.WriteSectSinceReboot
    );

    private static readonly Gauge<SystemResource> WrittenSectorsTotal = new(
        "mikrotik_written_sectors_total",
        "Total written sectors to HDD",
        i => i.WriteSectTotal
    );

    private static readonly Gauge<SystemResource> BadBlocks = new(
        "mikrotik_bad_blocks",
        "HDD bad blocks",
        i => i.BadBlocks
    );
}