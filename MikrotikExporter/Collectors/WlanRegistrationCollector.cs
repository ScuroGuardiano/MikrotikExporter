using System.Globalization;
using MikrotikApiClient;
using MikrotikApiClient.Dto;
using MikrotikExporter.Helpers;
using MikrotikExporter.PrometheusMappers;

namespace MikrotikExporter.Collectors;

/// <summary>
///
/// Frequency of change: medium I'd say, we have signal changes and transfer but do you care? If you care about accurate rates then it's high
/// </summary>
public class WlanRegistrationCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public WlanRegistrationCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true, CancellationToken cancellationToken = default)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }
        
        var registrationsTask = _client.GetWlanRegistrations(cancellationToken);
        var dhcpServerLeasesTask = _client.GetDhcpServerLeases(cancellationToken);
        
        await Task.WhenAll(registrationsTask, dhcpServerLeasesTask);
        
        var registrations = registrationsTask.Result;
        var dhcpServerLeases = dhcpServerLeasesTask.Result;
        
        return Map(registrations, dhcpServerLeases);
    }

    private MetricsCollection<WlanRegistration> Map(WlanRegistration[] registrations, DhcpServerLease[] leases)
    {
        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host
        };

        MetricsCollection<WlanRegistration> collection = new();
        collection.CreateValueSets(
            staticLabels,
            TxPackets, RxPackets, TxBytes, RxBytes,
            TxFrames, RxFrames, TxFrameBytes, RxFrameBytes,
            TxHwFrames, RxHwFrames, TxHwFrameBytes, RxHwFrameBytes,
            SignalStrengthCh0, SignalStrengthCh1,
            SignalStrengthCh2, SignalStrengthCh3,
            Uptime, LastActivity,
            TxCcq, PThroughput, SignalStrength, SignalToNoise
        );
        
        foreach (var registration in registrations)
        {
            Dictionary<string, string> labels = new()
            {
                ["id"] = registration.Id,
                ["interface"] = registration.Interface,
                ["mac_address"] = registration.MacAddress,
                ["ap"] = registration.Ap,
                ["wds"] = registration.Wds,
                ["bridge"] = registration.Bridge,
                ["management_protection"] = registration.ManagementProtection,
                ["wmm_enabled"] =  registration.WmmEnabled
            };
            
            // Find hostname
            var hostname = leases
                .FirstOrDefault(l => l.MacAddress == registration.MacAddress)
                ?.HostName;

            if (hostname is not null)
            {
                labels.AddIfNotEmpty("hostname", hostname);
            }
            
            labels.AddIfNotEmpty("rx_rate", registration.RxRate);
            labels.AddIfNotEmpty("tx_rate", registration.TxRate);
            labels.AddIfNotEmpty("last_ip",  registration.LastIp);
            labels.AddIfNotEmpty("port_802_1x_enabled", registration.Port802_1xEnabled);
            labels.AddIfNotEmpty("comment", registration.Comment);
            labels.AddIfNotEmpty("authentication_type", registration.AuthenticationType);
            labels.AddIfNotEmpty("encryption", registration.Encryption);
            labels.AddIfNotEmpty("group_encryption", registration.GroupEncryption);
            
            collection.AddValue(registration, labels);
        }
        
        return collection;
    }

    private static readonly Counter<WlanRegistration> TxPackets = new(
        "mikrotik_wireless_client_tx_packets_total",
        "Packets transmitted TO the wireless client",
        i => i.TxPackets
    );

    private static readonly Counter<WlanRegistration> RxPackets = new(
        "mikrotik_wireless_client_rx_packets_total",
        "Packets transmitted FROM the wireless client",
        i => i.RxPackets
    );

    private static readonly Counter<WlanRegistration> TxBytes = new(
        "mikrotik_wireless_client_tx_bytes_total",
        "Bytes transmitted TO the wireless client",
        i => i.TxBytes
    );

    private static readonly Counter<WlanRegistration> RxBytes = new(
        "mikrotik_wireless_client_rx_bytes_total",
        "Bytes transmitted FROM the wireless client",
        i => i.RxBytes
    );

    private static readonly Counter<WlanRegistration> TxFrames = new(
        "mikrotik_wireless_client_tx_frames_total",
        "Frames transmitted TO the wireless client",
        i => i.TxFrames
    );

    private static readonly Counter<WlanRegistration> RxFrames = new(
        "mikrotik_wireless_client_rx_frames_total",
        "Frames transmitted FROM the wireless client",
        i => i.RxFrames
    );

    private static readonly Counter<WlanRegistration> TxFrameBytes = new(
        "mikrotik_wireless_client_tx_frame_bytes_total",
        "Frame bytes transmitted TO the wireless client",
        i => i.TxFrameBytes
    );

    private static readonly Counter<WlanRegistration> RxFrameBytes = new(
        "mikrotik_wireless_client_rx_frame_bytes_total",
        "Frame bytes transmitted FROM the wireless client",
        i => i.RxFrameBytes
    );

    private static readonly Counter<WlanRegistration> TxHwFrames = new(
        "mikrotik_wireless_client_tx_hw_frames_total",
        "Hardware frames transmitted TO the wireless client",
        i => i.TxHwFrames
    );

    private static readonly Counter<WlanRegistration> RxHwFrames = new(
        "mikrotik_wireless_client_rx_hw_frames_total",
        "Hardware frames transmitted FROM the wireless client",
        i => i.RxHwFrames
    );

    private static readonly Counter<WlanRegistration> TxHwFrameBytes = new(
        "mikrotik_wireless_client_tx_hw_frame_bytes_total",
        "Hardware frame bytes transmitted TO the wireless client",
        i => i.TxHwFrameBytes
    );

    private static readonly Counter<WlanRegistration> RxHwFrameBytes = new(
        "mikrotik_wireless_client_rx_hw_frame_bytes_total",
        "Hardware frame bytes transmitted FROM the wireless client",
        i => i.RxHwFrameBytes
    );
    
    
    private static readonly Gauge<WlanRegistration> SignalStrengthCh0 = new(
        "mikrotik_wireless_client_signal_strength_ch0",
        "Signal strength on channel 0 (dBm)",
        i => double.TryParse(i.SignalStrengthCh0, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );

    private static readonly Gauge<WlanRegistration> SignalStrengthCh1 = new(
        "mikrotik_wireless_client_signal_strength_ch1",
        "Signal strength on channel 1 (dBm)",
        i => double.TryParse(i.SignalStrengthCh1, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );

    private static readonly Gauge<WlanRegistration> SignalStrengthCh2 = new(
        "mikrotik_wireless_client_signal_strength_ch2",
        "Signal strength on channel 2 (dBm)",
        i => double.TryParse(i.SignalStrengthCh2, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );

    private static readonly Gauge<WlanRegistration> SignalStrengthCh3 = new(
        "mikrotik_wireless_client_signal_strength_ch3",
        "Signal strength on channel 3 (dBm)",
        i => double.TryParse(i.SignalStrengthCh3, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );
    
    private static readonly Gauge<WlanRegistration> Uptime = new(
        "mikrotik_wireless_client_uptime_seconds",
        "Client uptime in seconds",
        i => MikrotikTimeSpanConverter.ToSeconds(i.Uptime).ToString(CultureInfo.InvariantCulture)
    );

    private static readonly Gauge<WlanRegistration> LastActivity = new(
        "mikrotik_wireless_client_last_activity_seconds",
        "Seconds since last activity",
        i => MikrotikTimeSpanConverter.ToSeconds(i.LastActivity).ToString(CultureInfo.InvariantCulture)
    );
    
    private static readonly Gauge<WlanRegistration> TxCcq = new(
        "mikrotik_wireless_client_tx_ccq",
        "Client Connection Quality (CCQ) in percentage",
        i => double.TryParse(i.TxCcq, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );

    private static readonly Gauge<WlanRegistration> PThroughput = new(
        "mikrotik_wireless_client_p_throughput",
        "Predicted throughput in bits per second",
        i => double.TryParse(i.PThroughput, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );

    private static readonly Gauge<WlanRegistration> SignalStrength = new(
        "mikrotik_wireless_client_signal_strength",
        "Overall signal strength in dBm",
        i => ParseSignal(i.SignalStrength)
    );

    private static readonly Gauge<WlanRegistration> SignalToNoise = new(
        "mikrotik_wireless_client_signal_to_noise",
        "Signal to noise ratio in dB",
        i => double.TryParse(i.SignalToNoise, out var val) ? val.ToString(CultureInfo.InvariantCulture) : null
    );
    
    private static string? ParseSignal(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        var parts = s.Split('@');
        return double.TryParse(parts[0], out var val) ? val.ToString(CultureInfo.InvariantCulture) : null;
    }
}