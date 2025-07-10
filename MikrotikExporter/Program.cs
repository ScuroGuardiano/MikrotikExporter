using MikrotikApiClient;
using MikrotikExporter.Collectors;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, MkJsonSerializerContext.Default);
});

builder.Services.AddResponseCompression();

builder.Services.AddMikrotikPooledApiClient(builder.Configuration);

builder.Services.AddScoped<MasterCollector>();

var app = builder.Build();

app.UseResponseCompression();

app.MapGet("/metrics", async (MasterCollector collector) => await collector.CollectAndStringify());

app.Run();
