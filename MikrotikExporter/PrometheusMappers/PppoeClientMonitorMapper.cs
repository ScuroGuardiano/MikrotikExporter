using MikrotikApiClient.Dto;

namespace MikrotikExporter.PrometheusMappers;

public static class PppoeClientMonitorMapper
{
    public static MetricsCollection Map(PppoeClientMonitor[] monitors, string routerName, string routerHost)
    {
        var collection = new MetricsCollection<PppoeClientMonitor>();
        var routerLabels = new Dictionary<string, string>
        {
            ["router"] = routerName, ["router_host"] = routerHost, ["type"] = "pppoe-client"
        };

        collection.CreateValueSets(
            routerLabels,
            UptimeSeconds, ActiveLinks, Mtu, Mru
        );

        foreach (var monitor in monitors)
        {
            Dictionary<string, string> labels = new()
            {
                ["name"] = monitor.Name,
                ["status"] = monitor.Status
            };
            
            labels.AddIfNotEmpty("encoding", monitor.Encoding);
            labels.AddIfNotEmpty("service_name", monitor.ServiceName);
            labels.AddIfNotEmpty("ac_name", monitor.AcName);
            labels.AddIfNotEmpty("ac_mac", monitor.AcMac);
            labels.AddIfNotEmpty("local_address", monitor.LocalAddress);
            labels.AddIfNotEmpty("remote_address", monitor.RemoteAddress);
            
            collection.AddValue(monitor, labels);
        }

        return collection;
    }

    private static readonly Gauge<PppoeClientMonitor> UptimeSeconds = new(
        "mikrotik_pppoe_client_uptime_seconds",
        "PPPoE client connection uptime in seconds",
        i => ParseUptimeToSeconds(i.Uptime)
    );

    private static readonly Gauge<PppoeClientMonitor> ActiveLinks = new(
        "mikrotik_pppoe_client_active_links",
        "Number of active PPPoE client links",
        i => i.ActiveLinks
    );

    private static readonly Gauge<PppoeClientMonitor> Mtu = new(
        "mikrotik_pppoe_client_mtu",
        "PPPoE client MTU",
        i => i.Mtu
    );

    private static readonly Gauge<PppoeClientMonitor> Mru = new(
        "mikrotik_pppoe_client_mru",
        "PPPoE client MRU",
        i => i.Mru
    );

    // Claude wrote this shit, I don't trust it but well, I guess it works
    // Except it can be cleaner
    // TODO: clean this AI mess
    private static string? ParseUptimeToSeconds(string uptime)
    {
        if (string.IsNullOrEmpty(uptime))
        {
            return null;
        }

        try
        {
            var totalSeconds = 0;
            var currentNumber = "";
            
            foreach (var c in uptime)
            {
                if (char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentNumber) && int.TryParse(currentNumber, out int value))
                    {
                        totalSeconds += c switch
                        {
                            'd' => value * 86400, // days
                            'h' => value * 3600,  // hours
                            'm' => value * 60,    // minutes
                            's' => value,         // seconds
                            _ => 0
                        };
                    }
                    currentNumber = "";
                }
            }
            
            // Handle case where string ends with a number but no unit (assume seconds)
            if (!string.IsNullOrEmpty(currentNumber) && int.TryParse(currentNumber, out int finalValue))
            {
                totalSeconds += finalValue;
            }

            return totalSeconds.ToString();
        }
        catch
        {
            return null;
        }
    }
}