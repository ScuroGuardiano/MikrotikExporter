using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

public interface IMikrotikApiClient
{
    public Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default);

    public Task<EtherInterfaceMonitor[]> GetEtherMonitor(IEnumerable<string> numbers,
        CancellationToken cancellationToken = default);
    
    public string Name { get; }
    public string Host { get; }
}
