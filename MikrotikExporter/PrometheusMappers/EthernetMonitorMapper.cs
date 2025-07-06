using MikrotikApiClient.Dto;

namespace MikrotikExporter.PrometheusMappers;

public static class EthernetMonitorMapper
{
    public static MetricsCollection Map(EthernetMonitor[] monitors, string routerName, string routerHost)
    {
        
        var collection = new MetricsCollection<EthernetMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = routerName, ["router_host"] = routerHost, ["type"] = "ether"
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
