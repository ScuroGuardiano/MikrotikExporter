using Microsoft.Extensions.Options;
using MikrotikApiClient.Dto;
using MikrotikApiClient.Tcp.Parsers;

namespace MikrotikApiClient.Tcp;

public sealed class MikrotikApiClient : IMikrotikApiClient, IDisposable
{
    private readonly MikrotikApiConnection _connection;
    private readonly MikrotikApiClientOptions _options;
    
    public string Host => _options.Host;
    public string Name => _options.Name ?? Host;

    public MikrotikApiClient(IOptions<MikrotikApiClientOptions> options)
    {
        _options = options.Value;
        _connection = new MikrotikApiConnection(_options.Host, _options.Username, _options.Password);
    }
    
    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/interface/print"], cancellationToken);
        res.EnsureSuccess();
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToInterfaceSummary())
            .ToArray();
    }

    public async Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request([
            "/interface/ethernet/monitor",
            "=once=",
            $"=numbers={string.Join(',', numbers)}"
        ], cancellationToken);

        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToEtherInterfaceMonitor())
            .ToArray();
    }

    public async Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request([
            "/interface/wireless/monitor",
            "=once=",
            $"=numbers={string.Join(',', numbers)}"
        ], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToWlanInterfaceMonitor())
            .ToArray();
    }

    public async Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request([
            "/interface/pppoe-client/monitor",
            "=once=",
            $"=numbers={string.Join(',', numbers)}"
        ],  cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToPppoeClientMonitor())
            .ToArray();
    }

    public async Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/system/health/print"], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToHealthStat())
            .ToArray();
    }

    public async Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/system/resource/print"], cancellationToken);

        // TODO: Make it cleaner after adding error handling XD
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToSystemResource())
            .First();
    }

    public async Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/dhcp-server/lease/print"], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToDhcpServerLease())
            .ToArray();
    }

    public async Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/firewall/connection/print"], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallConnection())
            .ToArray();
    }

    public async Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var raw = await _connection.Request(["/ip/firewall/raw/print"], cancellationToken);
        var filter = await _connection.Request(["/ip/firewall/filter/print"], cancellationToken);
        var mangle = await _connection.Request(["/ip/firewall/mangle/print"], cancellationToken);
        var nat = await _connection.Request(["/ip/firewall/nat/print"], cancellationToken);

        var rawParsed = raw.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallRule("raw"));
        var filterParsed = filter.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallRule("filter"));
        var mangleParsed = mangle.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallRule("mangle"));
        var natParsed = nat.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallRule("nat"));
        
        return rawParsed
            .Concat(filterParsed)
            .Concat(mangleParsed)
            .Concat(natParsed)
            .ToArray();
    }

    public async Task<IpPool[]> GetIpPools(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/pool/print"], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpPool())
            .ToArray();
    }

    public async Task<string> GetIdentity(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/system/identity/print"], cancellationToken);

        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.Attributes["name"])
            .FirstOrDefault(string.Empty);
    }

    public async Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/dns/cache/print"], cancellationToken);

        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToDnsRecord())
            .ToArray();
    }

    public async Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/interface/wireless/registration-table/print"], cancellationToken);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToWlanRegistration())
            .ToArray();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
