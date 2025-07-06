using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

public interface IMikrotikApiClient
{
    public Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default);

    public Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default);

    public Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default);

    public Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default);
    
    public Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default);
    
    public Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default);
    
    public Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default);
    
    public Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default);
    
    public string Name { get; }
    public string Host { get; }
}
