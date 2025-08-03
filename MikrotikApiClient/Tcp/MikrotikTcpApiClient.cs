using DotNext.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MikrotikApiClient.Dto;
using MikrotikApiClient.Tcp.Parsers;

namespace MikrotikApiClient.Tcp;

internal sealed class MikrotikTcpApiClient : IMikrotikApiClient
{
    private readonly MikrotikTcpApiConnection _connection;
    private readonly MikrotikApiClientOptions _options;
    
    public string Host => _options.Host;
    public string Name => _options.Name ?? Host;

    private readonly ILogger<MikrotikTcpApiClient> _logger;
    private readonly AsyncLazy<string?> _wirelessPackage;

    public MikrotikTcpApiClient(IOptions<MikrotikApiClientOptions> options, ILogger<MikrotikTcpApiClient> logger)
    {
        _logger = logger;
        _options = options.Value;
        _connection = new MikrotikTcpApiConnection(_options.Host, _options.Username, _options.Password, logger);
        
        _wirelessPackage = new AsyncLazy<string?>(async ct =>
        {
            var packages = (await GetPackages(ct))
                .Where(p => !p.Disabled)
                .Select(p => p.Name).ToArray();
            
            string? wirelessPackage = null;
            
            if (packages.Any(x => x.StartsWith("wireless", StringComparison.InvariantCultureIgnoreCase)))
            {
                wirelessPackage = "wireless";
            }
            else if (packages.Any(x => x.StartsWith("wifi", StringComparison.InvariantCultureIgnoreCase)))
            {
                wirelessPackage = "wifi";
            }
            else if (packages.Any(x => x.StartsWith("wifiwave2", StringComparison.InvariantCultureIgnoreCase)))
            {
                wirelessPackage = "wifiwave2";
            }

            if (wirelessPackage is null)
            {
                _logger.LogInformation("Wireless package not found. Wireless metrics won't be generated");
            }
            else
            {
                _logger.LogInformation("Wireless package: '{WirelessPackage}' found.", wirelessPackage);
            }
            
            return wirelessPackage;
        });
    }
    
    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/interface/print"]);
        res.EnsureSuccess(_logger);
        
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
        ]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToEtherInterfaceMonitor())
            .ToArray();
    }

    public async Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default)
    {
        var wirelessPackage = await _wirelessPackage.WithCancellation(cancellationToken);
        if (wirelessPackage == null)
        {
            return [];
        }
        
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request([
            $"/interface/{wirelessPackage}/monitor",
            "=once=",
            $"=numbers={string.Join(',', numbers)}"
        ]);
        
        res.EnsureSuccess(_logger);
        
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
        ]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToPppoeClientMonitor())
            .ToArray();
    }

    public async Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/system/health/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToHealthStat())
            .ToArray();
    }

    public async Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var res = await _connection.Request(["/system/resource/print"]);

        res.EnsureSuccess(_logger);
        
        var systemRes = res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToSystemResource())
            .FirstOrDefault();

        if (systemRes is null)
        {
            throw new MikrotikException("/system/resource/print", res.RequestSentence, "Received unexpected [empty] answer.");
        }
        
        return systemRes;
    }

    public async Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/dhcp-server/lease/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToDhcpServerLease())
            .ToArray();
    }

    public async Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/firewall/connection/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpFirewallConnection())
            .ToArray();
    }

    public async Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        
        var raw = await _connection.Request(["/ip/firewall/raw/print"]);
        var filter = await _connection.Request(["/ip/firewall/filter/print"]);
        var mangle = await _connection.Request(["/ip/firewall/mangle/print"]);
        var nat = await _connection.Request(["/ip/firewall/nat/print"]);
        
        raw.EnsureSuccess(_logger);
        filter.EnsureSuccess(_logger);
        mangle.EnsureSuccess(_logger);
        nat.EnsureSuccess(_logger);

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
        var res = await _connection.Request(["/ip/pool/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToIpPool())
            .ToArray();
    }

    public async Task<string> GetIdentity(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/system/identity/print"]);

        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.Attributes["name"])
            .FirstOrDefault(string.Empty);
    }

    public async Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/ip/dns/cache/print"]);

        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToDnsRecord())
            .ToArray();
    }

    public async Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
    {
        var wirelessPackage = await _wirelessPackage.WithCancellation(cancellationToken);
        if (wirelessPackage == null)
        {
            return [];
        }
        
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request([$"/interface/{wirelessPackage}/registration-table/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToWlanRegistration())
            .ToArray();
    }

    public async Task<Package[]> GetPackages(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureRunning(cancellationToken);
        var res = await _connection.Request(["/system/package/print"]);
        
        res.EnsureSuccess(_logger);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToPackage())
            .ToArray();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}