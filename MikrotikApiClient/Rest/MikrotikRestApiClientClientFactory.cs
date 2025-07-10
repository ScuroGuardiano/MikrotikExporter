using Microsoft.Extensions.Options;

namespace MikrotikApiClient.Rest;

internal class MikrotikRestApiClientClientFactory : IMikrotikApiClientFactory
{
    private readonly IOptions<MikrotikApiClientOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public MikrotikRestApiClientClientFactory(IOptions<MikrotikApiClientOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    public IMikrotikApiClient CreateClient()
    {
        return new MikrotikRestApiClient(_httpClientFactory.CreateClient(), _options);
    }
}
