using System.Diagnostics;
using MikrotikApiClient;
using MikrotikExporter.PrometheusMappers;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, MkJsonSerializerContext.Default);
});

builder.Services.AddOptions<MikrotikApiClientOptions>()
    .Bind(builder.Configuration.GetSection("Mikrotik"));

// builder.Services.AddHttpClient<IMikrotikApiClient, MikrotikRestApiClient>();
builder.Services.AddScoped<IMikrotikApiClient, MikrotikApiClient.Tcp.MikrotikApiClient>();

var app = builder.Build();

app.MapGet("/interface", async (IMikrotikApiClient client) => await client.GetInterfaces());

app.MapGet("/metrics", async (IMikrotikApiClient client) =>
{
    var interfaces =  await client.GetInterfaces();
    return InterfaceSummaryMapper.Map(interfaces, client.Name, client.Host);
    
});

app.Run();
