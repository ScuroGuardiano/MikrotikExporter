using DotNext.Threading;
using Microsoft.Extensions.DependencyInjection;
using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

public class MikrotikConcurrentCachedApiClient : IMikrotikConcurrentApiClient
{
    private readonly IMikrotikApiClient _client;

    public MikrotikConcurrentCachedApiClient([FromKeyedServices("OriginalClient")] IMikrotikConcurrentApiClient client)
    {
        _client = client;
            
        _interfaceSummaries = new AsyncLazy<InterfaceSummary[]>(ct => _client.GetInterfaces(ct));
        _dhcpLeases = new AsyncLazy<DhcpServerLease[]>(ct => _client.GetDhcpServerLeases(ct));
        _healthStats = new AsyncLazy<HealthStat[]>(ct => _client.GetHealthStats(ct));
        _systemResource = new AsyncLazy<SystemResource>(ct => _client.GetSystemResource(ct));
        _firewallConnections = new AsyncLazy<IpFirewallConnection[]>(ct => _client.GetIpFirewallConnections(ct));
        _firewallRules = new AsyncLazy<IpFirewallRule[]>(ct => _client.GetIpFirewallRules(ct));
        _ipPools = new AsyncLazy<IpPool[]>(ct => _client.GetIpPools(ct));
        _identity = new AsyncLazy<string>(ct => _client.GetIdentity(ct));
        _dnsCacheRecords = new AsyncLazy<DnsCacheRecord[]>(ct => _client.GetDnsCacheRecords(ct));
        _wlanRegistrations = new AsyncLazy<WlanRegistration[]>(ct => _client.GetWlanRegistrations(ct));
        _packages = new AsyncLazy<Package[]>(ct => _client.GetPackages(ct));
    }

    public string Name => _client.Name;
    public string Host => _client.Host;

    private readonly AsyncLazy<InterfaceSummary[]> _interfaceSummaries;
    private readonly AsyncLazy<DhcpServerLease[]> _dhcpLeases;
    private readonly AsyncLazy<HealthStat[]> _healthStats;
    private readonly AsyncLazy<SystemResource> _systemResource;
    private readonly AsyncLazy<IpFirewallConnection[]> _firewallConnections;
    private readonly AsyncLazy<IpFirewallRule[]> _firewallRules;
    private readonly AsyncLazy<IpPool[]> _ipPools;
    private readonly AsyncLazy<string> _identity;
    private readonly AsyncLazy<DnsCacheRecord[]> _dnsCacheRecords;
    private readonly AsyncLazy<WlanRegistration[]> _wlanRegistrations;
    private readonly AsyncLazy<Package[]> _packages;

    public Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
        => _interfaceSummaries.WithCancellation(cancellationToken);

    public Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
        => _dhcpLeases.WithCancellation(cancellationToken);

    public Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
        => _healthStats.WithCancellation(cancellationToken);

    public Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
        => _systemResource.WithCancellation(cancellationToken);

    public Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
        => _firewallConnections.WithCancellation(cancellationToken);

    public Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
        => _firewallRules.WithCancellation(cancellationToken);

    public Task<IpPool[]> GetIpPools(CancellationToken cancellationToken = default)
        => _ipPools.WithCancellation(cancellationToken);

    public Task<string> GetIdentity(CancellationToken cancellationToken = default)
        => _identity.WithCancellation(cancellationToken);

    public Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
        => _dnsCacheRecords.WithCancellation(cancellationToken);

    public Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
        => _wlanRegistrations.WithCancellation(cancellationToken);

    // Three values below are not cached but whatever man. They are only used one within request
    public Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
        => _client.GetEtherMonitor(numbers, cancellationToken); // not cached due to argument dependence

    public Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
        => _client.GetWlanMonitor(numbers, cancellationToken); // not cached due to argument dependence

    public Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
        => _client.GetPppoeClientMonitor(numbers, cancellationToken); // not cached due to argument dependence.

    public Task<Package[]> GetPackages(CancellationToken cancellationToken = default)
        => _packages.WithCancellation(cancellationToken);

    public void Dispose()
    {
        // DO NOT
        // DISPOSE
        // FUCKING INJECTED SERVICE
        // GOD FUCKING DAMN DELULU CHATGPT INSERTED BELOW LINES HERE:
        
        // if (_client is IDisposable disposable)
        // {
        // }
    }
}
