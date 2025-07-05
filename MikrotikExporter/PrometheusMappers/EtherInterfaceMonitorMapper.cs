using MikrotikApiClient.Dto;

namespace MikrotikExporter.PrometheusMappers;

public class EtherInterfaceMonitorMapper
{
    public static MetricsCollection Map(EtherInterfaceMonitor[] monitors, string routerName, string routerHost)
    {
        
        var collection = new MetricsCollection<EtherInterfaceMonitor>();
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

    private static Gauge<EtherInterfaceMonitor> SfpWavelength = new(
        "sfp-wavelength",
        "Wavelength of laser",
        i => i.SfpWavelength
    );

    private static Gauge<EtherInterfaceMonitor> SfpTemperature = new(
        "sfp-temperature",
        "Temperature of SFP stick",
        i => i.SfpTemperature
    );

    private static Gauge<EtherInterfaceMonitor> SfpSupplyVoltage = new(
        "sfp-supply-voltage",
        "Supply voltage of SFP",
        i => i.SfpSupplyVoltage
    );

    private static Gauge<EtherInterfaceMonitor> SfpTxBiasCurrent = new(
        "sfp-tx-bias-current",
        "SFP Bias current",
        i => i.SfpTxBiasCurrent
    );

    private static Gauge<EtherInterfaceMonitor> SfpTxPower = new (
        "sfp-tx-power",
        "SFP TX Power",
        i => i.SfpTxPower
    );

    private static Gauge<EtherInterfaceMonitor> SfpRxPower = new(
        "sfp-rx-power",
        "SFP RX Power",
        i => i.SfpRxPower
    );
}