using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MikrotikApiClient.Rest;
using MikrotikApiClient.Tcp;

namespace MikrotikApiClient;

public static class MikrotikServiceCollectionExtensions
{
    public static void AddMikrotikPooledApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MikrotikApiClientOptions>()
            .Bind(configuration.GetSection("Mikrotik"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        if (configuration.GetSection("Mikrotik").GetValue<string>("Host") is not { } host)
        {
            throw new Exception("Mikrotik host is missing in the 'Mikrotik' configuration section. Ensure 'Host' is specified in appsettings.json or MIKROTIK__HOST environment variable.");
        }
        
        if (host.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHttpClient();
                
            // C# is already doing HTTP Pooling and HttpClients should be short-lived I guess
            // So we add all those services as scoped per-request
            // The only reason I add pool here is because I want to make multiple requests concurrently
            services.AddScoped<IMikrotikApiClientFactory, MikrotikRestApiClientClientFactory>();
            services.AddScoped<IMikrotikApiClient, MikrotikApiClientPool>();
        }
        else
        {
            // In case of TCP I want to keep connection all the time and limit concurrent connections
            // for an entire application. It helps to
            // 1. Limit maximum load on router
            // 2. Saves few milliseconds on login request and tcp handshake
            services.AddSingleton<IMikrotikApiClientFactory, MikrotikTcpApiClientClientFactory>();
            services.AddSingleton<IMikrotikApiClient, MikrotikApiClientPool>();
        }
    }
}
