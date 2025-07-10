namespace MikrotikApiClient;

internal interface IMikrotikApiClientFactory
{
    public IMikrotikApiClient CreateClient();
}
