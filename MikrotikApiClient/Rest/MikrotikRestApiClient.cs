using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Rest;

internal sealed class MikrotikRestApiClient : IMikrotikApiClient
{
    private readonly HttpClient _httpClient;
    private readonly MikrotikApiClientOptions _options;

    public string Host => _httpClient.BaseAddress?.Host ?? "";
    public Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IpPool[]> GetIpPools(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetIdentity(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Package[]> GetPackages(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string Name => _options.Name ?? Host;

    public MikrotikRestApiClient(HttpClient httpClient, IOptions<MikrotikApiClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        
        var uri = new Uri(_options.Host + "/rest/");
        
        httpClient.BaseAddress = uri;

        var authData = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}")
        );
        
        httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {authData}");
    }

    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        if (await _httpClient.GetFromJsonAsync(
                "interface",
                typeof(InterfaceSummary[]),
                MkJsonSerializerContext.Default,
                cancellationToken: cancellationToken) is not InterfaceSummary[] res
           )
        {
            throw new Exception("Received unexpected response from the router.");
        }
        
        return res;
    }

    public void Dispose()
    {
        
    }
}