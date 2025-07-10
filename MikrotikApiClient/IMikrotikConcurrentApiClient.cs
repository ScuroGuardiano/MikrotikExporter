namespace MikrotikApiClient;

/// <summary>
/// Just like IMikrotikApiClient but can be used from multiple threads at the same time.
/// </summary>
public interface IMikrotikConcurrentApiClient : IMikrotikApiClient
{
}
