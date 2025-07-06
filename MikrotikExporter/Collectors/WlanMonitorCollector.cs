using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

public class WlanMonitorCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;

    public WlanMonitorCollector(IMikrotikApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(InterfaceSummary[] interfaces)
    {
        if (!Enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var ifaces = interfaces.Where(i => i.Type == "wlan").ToArray();
        
        var monitors = await _client.GetWlanMonitor(ifaces.Select(i => i.Id));
        
        for (var i = 0; i < Math.Min(monitors.Length, ifaces.Length); i++)
        {
            monitors[i].Name = ifaces[i].Name;
        }
        
        return WlanMonitorMapper.Map(monitors, _client.Name, _client.Host);
    }
}