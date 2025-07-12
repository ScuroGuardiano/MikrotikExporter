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
    private readonly IpFirewallRuleCollector _ipFirewallRuleCollector;
    private readonly IpFirewallConnectionCollector _ipFirewallConnectionCollector;
    private readonly WlanRegistrationCollector _wlanRegistrationCollector;
    private readonly HealthCollector _healthCollector;
    private readonly SystemResourceCollector _systemResourceCollector;

    public MasterCollector(IMikrotikConcurrentApiClient client)
    {
        _summaryCollector = new InterfaceSummaryCollector(client);
        _etherMonitorCollector = new EthernetMonitorCollector(client);
        _collectTimeCollector = new CollectTimeCollector(client);
        _wlanMonitorCollector = new WlanMonitorCollector(client);
        _pppoeClientMonitorCollector = new PppoeClientMonitorCollector(client);
        _routerInfoCollector = new RouterInfoCollector(client);
        _dhcpServerLeaseCollector = new DhcpServerLeaseCollector(client);
        _ipPoolCollector = new IpPoolCollector(client);
        _ipFirewallRuleCollector = new IpFirewallRuleCollector(client);
        _ipFirewallConnectionCollector = new IpFirewallConnectionCollector(client);
        _wlanRegistrationCollector = new WlanRegistrationCollector(client);
        _healthCollector = new HealthCollector(client);
        _systemResourceCollector = new SystemResourceCollector(client);
    }

    public async Task<string> CollectAndStringify()
    {
        _collectTimeCollector.Start();
        
        List<Task<MetricsCollection>> collectionsTasks =
        [
            _summaryCollector.Collect(),
            _etherMonitorCollector.Collect(),
            _wlanMonitorCollector.Collect(),
            _pppoeClientMonitorCollector.Collect(),
            _dhcpServerLeaseCollector.Collect(),
            _ipPoolCollector.Collect(),
            _ipFirewallRuleCollector.Collect(),
            _ipFirewallConnectionCollector.Collect(),
            _wlanRegistrationCollector.Collect(),
            _healthCollector.Collect(),
            _routerInfoCollector.Collect(),
            _systemResourceCollector.Collect()
        ];
        
        await Task.WhenAll(collectionsTasks);
        var collectionTime = _collectTimeCollector.Collect();
        
        var collections = collectionsTasks.Select(t => t.Result).ToList();
        collections.Add(collectionTime);
        
        return string.Join('\n', collections.Select(c => c.ToString()));
    }

    public async Task<string> CollectAndStringify(IReadOnlySet<Type> enableList)
    {
        _collectTimeCollector.Start();
        
        // Embrace mess, it's my mess.
        
        List<Task<MetricsCollection>> collectionsTasks =
        [
            _summaryCollector.Collect(enableList.Contains(_summaryCollector.GetType())),
            _etherMonitorCollector.Collect(enableList.Contains(_etherMonitorCollector.GetType())),
            _wlanMonitorCollector.Collect(enableList.Contains(_wlanMonitorCollector.GetType())),
            _pppoeClientMonitorCollector.Collect(enableList.Contains(_pppoeClientMonitorCollector.GetType())),
            _dhcpServerLeaseCollector.Collect(enableList.Contains(_dhcpServerLeaseCollector.GetType())),
            _ipPoolCollector.Collect(enableList.Contains(_ipPoolCollector.GetType())),
            _ipFirewallRuleCollector.Collect(enableList.Contains(_ipFirewallRuleCollector.GetType())),
            _ipFirewallConnectionCollector.Collect(enableList.Contains(_ipFirewallConnectionCollector.GetType())),
            _wlanRegistrationCollector.Collect(enableList.Contains(_wlanRegistrationCollector.GetType())),
            _healthCollector.Collect(enableList.Contains(_healthCollector.GetType())),
            _routerInfoCollector.Collect(enableList.Contains(_routerInfoCollector.GetType())),
            _systemResourceCollector.Collect(enableList.Contains(_systemResourceCollector.GetType()))
        ];
        
        await Task.WhenAll(collectionsTasks);
        var collectionTime = _collectTimeCollector.Collect();
        
        var collections = collectionsTasks.Select(t => t.Result).ToList();
        collections.Add(collectionTime);
        
        return string.Join(
            '\n',
            collections
                .Select(c => c.ToString())
                .Where(s => !string.IsNullOrEmpty(s))
        );
    }
}