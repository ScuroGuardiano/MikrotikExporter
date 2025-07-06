using MikrotikApiClient;

namespace MikrotikExporter.Collectors;

public class MasterCollector
{
    private readonly InterfaceSummaryCollector _summaryCollector;
    private readonly EthernetMonitorCollector _etherMonitorCollector;
    private readonly CollectTimeCollector _collectTimeCollector;

    public MasterCollector(IMikrotikApiClient client)
    {
        _summaryCollector = new InterfaceSummaryCollector(client);
        _etherMonitorCollector = new EthernetMonitorCollector(client);
        _collectTimeCollector = new CollectTimeCollector(client);
    }

    public async Task<string> CollectAndStringify()
    {
        _collectTimeCollector.Start();
        
        var (interfaceCollection, interfaces) = await _summaryCollector.CollectWithInterfaces();

        List<MetricsCollection> collections =
        [
            interfaceCollection,
            await _etherMonitorCollector.Collect(interfaces),

            _collectTimeCollector.Collect()
        ];

        return string.Join("", collections.Select(c => c.ToString()));
    }
}
