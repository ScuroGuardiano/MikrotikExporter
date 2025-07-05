using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

public class EtherInterfaceMonitorCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public EtherInterfaceMonitorCollector(IMikrotikApiClient client)
    {
        _client = client;
    }
    
    public async Task<MetricsCollection> Collect(InterfaceSummary[] interfaces)
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var etherMonitors = await _client.GetEtherMonitor(
            interfaces.Where(i => i.Running == "true")
                .Where(i => i.Type == "ether")
                .Where(i => i.DefaultName.StartsWith("sfp"))
                .Select(i => i.Id)
        );

        return EtherInterfaceMonitorMapper.Map(etherMonitors, _client.Name, _client.Host);
    }
}
