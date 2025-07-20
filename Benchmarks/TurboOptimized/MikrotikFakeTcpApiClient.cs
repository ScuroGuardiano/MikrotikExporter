using Benchmarks.Dto;
using Benchmarks.TurboOptimized.Parsers;

namespace Benchmarks.TurboOptimized;

internal sealed class MikrotikFakeTcpApiClient : IMikrotikApiClient
{
    private readonly MikrotikFakeTcpApiConnection _connection = new();
    
    public string Host => "fake.uwu";
    public string Name => "Fake" ?? Host;

    
    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        using var res = await _connection.Request(["/interface/print"u8.ToArray()], cancellationToken);
        res.EnsureSuccess();

        var ret = new List<InterfaceSummary>(res.Sentences.Count);

        foreach (var s in res.Sentences)
        {
            if (s.Reply == "!re")
            {
                ret.Add(s.ToInterfaceSummary());
            }
        }
        
        return ret.ToArray();
    }

    public async Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IpPool[]> GetIpPools(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetIdentity(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
