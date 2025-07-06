using System.Net.Sockets;
using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class PppoeClientMonitorParser
{
    public static PppoeClientMonitor ToPppoeClientMonitor(this MikrotikSentence s)
    {
        
        return new PppoeClientMonitor()
        {
            Status = s.Attributes.GetValueOrDefault("status", string.Empty),
            Uptime = s.Attributes.GetValueOrDefault("uptime", string.Empty),
            ActiveLinks = s.Attributes.GetValueOrDefault("active-links", "0"),
            Encoding = s.Attributes.GetValueOrDefault("encoding", string.Empty),
            ServiceName = s.Attributes.GetValueOrDefault("service-name", string.Empty),
            AcName = s.Attributes.GetValueOrDefault("ac-name", string.Empty),
            AcMac = s.Attributes.GetValueOrDefault("ac-mac", string.Empty),
            Mtu = s.Attributes.GetValueOrDefault("mtu", "0"),
            Mru = s.Attributes.GetValueOrDefault("mru", "0"),
            LocalAddress = s.Attributes.GetValueOrDefault("local-address", string.Empty),
            RemoteAddress = s.Attributes.GetValueOrDefault("remote-address", string.Empty)
        };
    }
}