using System.Xml;
using MikrotikApiClient;
using MikrotikExporter;
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

// Presets
app.MapGet("/metrics/presets/low-freq", async (MasterCollector collector)
    => await collector.CollectAndStringify(CollectionPresets.LowFrequencyOfChange));

app.MapGet("/metrics/presets/medium-freq", async (MasterCollector collector)
    => await collector.CollectAndStringify(CollectionPresets.MediumFrequencyOfChange));

app.MapGet("/metrics/presets/high-freq", async (MasterCollector collector)
    => await collector.CollectAndStringify(CollectionPresets.HighFrequencyOfChange));

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

app.MapGet("/metrics/{*selected}", async (MasterCollector collector, string selected) =>
{ 
    var mapped = selected.Split('/', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => collectorsMap.GetValueOrDefault(s))
        .Where(s => s is not null)
        .Select(s => s!) // To makes nulls go away from type :(
        .ToHashSet();

    if (mapped.Count == 0)
    {
        return Results.BadRequest("No valid metrics selected");
    }
    
    return Results.Ok(await collector.CollectAndStringify(mapped));
});

app.Run();
