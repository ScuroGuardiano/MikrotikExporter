using System.Diagnostics;
using MikrotikApiClient;
using MikrotikExporter.Collectors;
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
builder.Services.AddScoped<MasterCollector>();

var app = builder.Build();

app.MapGet("/interface", async (IMikrotikApiClient client) => await client.GetInterfaces());

app.MapGet("/metrics", async (MasterCollector collector) => await collector.CollectAndStringify());

app.Run();
