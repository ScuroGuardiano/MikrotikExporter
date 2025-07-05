using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
/// This collector returns MetricsCollection and optionally interface summary list
/// As this list will be needed for others collectors ;3
/// </summary>
public class InterfaceSummaryCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public InterfaceSummaryCollector(IMikrotikApiClient client)
    {
        _client = client;
    }
    
    public async Task<MetricsCollection> Collect()
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }

        return InterfaceSummaryMapper.Map(await _client.GetInterfaces(), _client.Name, _client.Host);
    }

    /// <summary>
    /// This method returns tuple of metrics and interface summary array.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public async Task<(MetricsCollection, InterfaceSummary[])> CollectWithInterfaces()
    {
        if (!Enabled)
        {
            return (MetricsCollection.Empty, []);
        }
        
        var interfaces = await _client.GetInterfaces();
        return (InterfaceSummaryMapper.Map(interfaces, _client.Name, _client.Host), interfaces);
    }
}
