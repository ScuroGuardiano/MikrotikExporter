using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MikrotikApiClient.Tcp;

internal class MikrotikTcpApiClientClientFactory : IMikrotikApiClientFactory
{
    private readonly IOptions<MikrotikApiClientOptions> _options;
    private readonly ILoggerFactory _loggerFactory;

    public MikrotikTcpApiClientClientFactory(IOptions<MikrotikApiClientOptions> options, ILoggerFactory loggerFactory)
    {
        _options = options;
        _loggerFactory = loggerFactory;
        _loggerFactory = loggerFactory;
    }


    public IMikrotikApiClient CreateClient()
    {
        var logger = _loggerFactory.CreateLogger<MikrotikTcpApiClient>();
        return new MikrotikTcpApiClient(_options, logger);
    }
}
