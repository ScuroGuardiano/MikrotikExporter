using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class WlanMonitorParser
{
    public static WlanMonitor ToWlanInterfaceMonitor(this MikrotikSentence s)
    {
        return new WlanMonitor
        {
            // Name has to be injected from external source (e.g., based on =.id= or =numbers= param order)
            // Example: Name = externalNameProvider[index]
            Status = s.Attributes.GetValueOrDefault("status", string.Empty),
            Channel = s.Attributes.GetValueOrDefault("channel", string.Empty),
            WirelessProtocol = s.Attributes.GetValueOrDefault("wireless-protocol", string.Empty),
            NoiseFloor = s.Attributes.GetValueOrDefault("noise-floor", "0"),
            OverallTxCcq = s.Attributes.GetValueOrDefault("overall-tx-ccq", "0"),
            RegisteredClients = s.Attributes.GetValueOrDefault("registered-clients", "0"),
            AuthenticatedClients = s.Attributes.GetValueOrDefault("authenticated-clients", "0"),
            WmmEnabled = s.Attributes.GetValueOrDefault("wmm-enabled", "false"),
            NotifyExternalFdb = s.Attributes.GetValueOrDefault("notify-external-fdb", "false")
        };
    }
}
