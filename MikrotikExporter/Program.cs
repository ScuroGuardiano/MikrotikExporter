using Microsoft.AspNetCore.Diagnostics;
using MikrotikApiClient;
using MikrotikExporter;
using MikrotikExporter.Collectors;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, MkJsonSerializerContext.Default);
});

builder.Logging.AddSimpleConsole();

builder.Services.AddResponseCompression();

builder.Services.AddMikrotikPooledApiClient(builder.Configuration);
builder.Services.AddScoped<MasterCollector>();

var app = builder.Build();

app.UseResponseCompression();

app.UseExceptionHandler(handlerBuilder =>
{
    handlerBuilder.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is MikrotikException)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/plain";
            
            await context.Response.WriteAsync(exception.Message);
        }
    });
});

app.MapGet("/metrics", async (MasterCollector collector, CancellationToken cancellationToken)
    => await collector.CollectAndStringify(cancellationToken));

// Presets
app.MapGet("/metrics/presets/low-freq", async (MasterCollector collector, CancellationToken cancellationToken)
    => await collector.CollectAndStringify(CollectionPresets.LowFrequencyOfChange, cancellationToken));

app.MapGet("/metrics/presets/medium-freq", async (MasterCollector collector, CancellationToken cancellationToken)
    => await collector.CollectAndStringify(CollectionPresets.MediumFrequencyOfChange, cancellationToken));

app.MapGet("/metrics/presets/high-freq", async (MasterCollector collector, CancellationToken cancellationToken)
    => await collector.CollectAndStringify(CollectionPresets.HighFrequencyOfChange, cancellationToken));

app.MapGet("/metrics/presets/{*preset}", (string preset) =>
{
    Results.NotFound($"Preset {preset} does not exists.");
});

// Only specific metrics

var collectorsMap = new Dictionary<string, Type>()
{
    ["ifaces"] = typeof(InterfaceSummaryCollector),
    ["ether-mon"] = typeof(EthernetMonitorCollector),
    ["wlan-mon"] = typeof(WlanMonitorCollector),
    ["health"] = typeof(HealthCollector),
    ["info"] = typeof(RouterInfoCollector),
    ["resources"] = typeof(SystemResourceCollector),
    ["dhcp-server-leases"] = typeof(DhcpServerLeaseCollector),
    ["ip-pool"] = typeof(IpPoolCollector),
    ["pppoe-client-mon"] = typeof(PppoeClientMonitorCollector),
    ["wlan-registration"] = typeof(WlanRegistrationCollector),
    ["firewall-rules"] = typeof(IpFirewallRuleCollector),
    ["firewall-conn"] = typeof(IpFirewallConnectionCollector)
}.AsReadOnly();

app.MapGet("/metrics/{*selected}", async (MasterCollector collector, string selected, CancellationToken cancellationToken) =>
{ 
    var mapped = selected.Split('/', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => collectorsMap.GetValueOrDefault(s))
        .Where(s => s is not null)
        .Select(s => s!) // To makes nulls go away from type :(
        .ToHashSet();

    if (mapped.Count == 0)
    {
        return Results.NotFound("No valid metrics selected");
    }
    
    return Results.Text(await collector.CollectAndStringify(mapped, cancellationToken));
});

app.Run();
