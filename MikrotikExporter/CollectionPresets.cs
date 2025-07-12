using MikrotikExporter.Collectors;

namespace MikrotikExporter;

public static class CollectionPresets
{
    /// <summary>
    /// Metrics with low frequency of change, collect them like every 20-30 seconds I guess
    /// </summary>
    public static IReadOnlySet<Type> LowFrequencyOfChange { get; } = new HashSet<Type>()
    {
        typeof(DhcpServerLeaseCollector),
        typeof(IpPoolCollector),
        typeof(PppoeClientMonitorCollector),
        typeof(IpFirewallRuleCollector)
    };

    /// <summary>
    /// Metrics with medium frequency of change, collect them like every 10 seconds I guess
    /// </summary>
    public static IReadOnlySet<Type> MediumFrequencyOfChange { get; } = new HashSet<Type>()
    {
        typeof(WlanMonitorCollector),
        typeof(EthernetMonitorCollector),
        typeof(WlanRegistrationCollector),
        typeof(HealthCollector)
    };

    /// <summary>
    /// Metrics with high frequency of change, I plan to collect them every seconds
    /// </summary>
    public static IReadOnlySet<Type> HighFrequencyOfChange { get; } = new HashSet<Type>()
    {
        typeof(InterfaceSummaryCollector),
        typeof(IpFirewallConnectionCollector),
        typeof(SystemResourceCollector),
    };
}
