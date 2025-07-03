using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

public interface IMikrotikApiClient
{
    public Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default);
    
    public string Name { get; }
    public string Host { get; }
}
