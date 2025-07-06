using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

public class EthernetMonitorCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public EthernetMonitorCollector(IMikrotikApiClient client)
    {
        _client = client;
    }
    
    public async Task<MetricsCollection> Collect(InterfaceSummary[] interfaces)
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }
        
        // I am only fetching metrics for SFP
        // because ether interfaces does not have anything interesting in them
        // at least for me. Change my mind and I will enable them ^^
        var etherMonitors = await _client.GetEtherMonitor(
            interfaces.Where(i => i.Running == "true")
                .Where(i => i.Type == "ether")
                .Where(i => i.DefaultName.StartsWith("sfp"))
                .Select(i => i.Id)
        );

        return EthernetMonitorMapper.Map(etherMonitors, _client.Name, _client.Host);
    }
}
