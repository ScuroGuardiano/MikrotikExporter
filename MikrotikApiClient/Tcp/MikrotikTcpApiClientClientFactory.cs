using Microsoft.Extensions.Options;

namespace MikrotikApiClient.Tcp;

internal class MikrotikTcpApiClientClientFactory : IMikrotikApiClientFactory
{
    private readonly IOptions<MikrotikApiClientOptions> _options;

    public MikrotikTcpApiClientClientFactory(IOptions<MikrotikApiClientOptions> options)
    {
        _options = options;
    }


    public IMikrotikApiClient CreateClient()
    {
        return new MikrotikTcpApiClient(_options);
    }
}
