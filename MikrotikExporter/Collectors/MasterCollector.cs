using MikrotikApiClient;

namespace MikrotikExporter.Collectors;

public class MasterCollector
{
    private readonly InterfaceSummaryCollector _summaryCollector;
    private readonly EthernetMonitorCollector _etherMonitorCollector;
    private readonly CollectTimeCollector _collectTimeCollector;
    private readonly WlanMonitorCollector _wlanMonitorCollector;
    private readonly PppoeClientMonitorCollector _pppoeClientMonitorCollector;
    private readonly RouterInfoCollector _routerInfoCollector;
    private readonly DhcpServerLeaseCollector _dhcpServerLeaseCollector;
    private readonly IpPoolCollector _ipPoolCollector;

    public MasterCollector(IMikrotikApiClient client)
    {
        _summaryCollector = new InterfaceSummaryCollector(client);
        _etherMonitorCollector = new EthernetMonitorCollector(client);
        _collectTimeCollector = new CollectTimeCollector(client);
        _wlanMonitorCollector = new WlanMonitorCollector(client);
        _pppoeClientMonitorCollector = new PppoeClientMonitorCollector(client);
        _routerInfoCollector = new RouterInfoCollector(client);
        _dhcpServerLeaseCollector = new DhcpServerLeaseCollector(client);
        _ipPoolCollector = new IpPoolCollector(client);
    }

    public async Task<string> CollectAndStringify()
    {
        _collectTimeCollector.Start();
        
        var (interfaceCollection, interfaces) = await _summaryCollector.CollectWithInterfaces();

        List<MetricsCollection> collections =
        [
            interfaceCollection,
            await _etherMonitorCollector.Collect(interfaces),
            await _wlanMonitorCollector.Collect(interfaces),
            await _pppoeClientMonitorCollector.Collect(interfaces),
            ..await _routerInfoCollector.Collect(),
            await _dhcpServerLeaseCollector.Collect(),
            await _ipPoolCollector.Collect(),

            _collectTimeCollector.Collect()
        ];

        return string.Join('\n', collections.Select(c => c.ToString()));
    }
}
