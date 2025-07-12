using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikApiClient.Tcp;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

public class EthernetMonitorCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public EthernetMonitorCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }
    
    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var interfaces = await _client.GetInterfaces();
        
        // I am only fetching metrics for SFP
        // because ether interfaces does not have anything interesting in them
        // at least for me. Change my mind and I will enable them ^^
        var etherMonitors = await _client.GetEtherMonitor(
            interfaces.Where(i => i.Running == "true")
                .Where(i => i.Type == "ether")
                .Where(i => i.DefaultName.StartsWith("sfp"))
                .Select(i => i.Id)
        );

        return Map(etherMonitors);
    }
    
    private MetricsCollection<EthernetMonitor> Map(EthernetMonitor[] monitors)
    {
        
        var collection = new MetricsCollection<EthernetMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = _client.Name, ["router_host"] = _client.Host, ["type"] = "ether"
        };

        collection.CreateValueSets(
            routerLabels,
            SfpWavelength, SfpTemperature, SfpSupplyVoltage,
            SfpTxBiasCurrent, SfpTxPower, SfpRxPower
        );

        foreach (var monitor in monitors)
        {
            Dictionary<string, string> labels = new()
            {
                ["name"] = monitor.Name,
                ["status"] = monitor.Status
            };
            
            collection.AddValue(monitor, labels);
        }

        return collection;
    }
    
    private static readonly Gauge<EthernetMonitor> SfpWavelength = new(
        "mikrotik_sfp_wavelength",
        "Wavelength of laser",
        i => i.SfpWavelength
    );

    private static readonly Gauge<EthernetMonitor> SfpTemperature = new(
        "mikrotik_sfp_temperature",
        "Temperature of SFP stick",
        i => i.SfpTemperature
    );

    private static readonly Gauge<EthernetMonitor> SfpSupplyVoltage = new(
        "mikrotik_sfp_supply_voltage",
        "Supply voltage of SFP",
        i => i.SfpSupplyVoltage
    );

    private static readonly Gauge<EthernetMonitor> SfpTxBiasCurrent = new(
        "mikrotik_sfp_tx_bias_current",
        "SFP Bias current",
        i => i.SfpTxBiasCurrent
    );

    private static readonly Gauge<EthernetMonitor> SfpTxPower = new (
        "mikrotik_sfp_tx_power",
        "SFP TX Power",
        i => i.SfpTxPower
    );

    private static readonly Gauge<EthernetMonitor> SfpRxPower = new(
        "mikrotik_sfp_rx_power",
        "SFP RX Power",
        i => i.SfpRxPower
    );
}
