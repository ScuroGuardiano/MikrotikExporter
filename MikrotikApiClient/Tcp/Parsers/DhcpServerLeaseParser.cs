using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class DhcpServerLeaseParser
{
    public static DhcpServerLease ToDhcpServerLease(this MikrotikSentence s)
    {
        return new DhcpServerLease
        {
            Id = s.Attributes[".id"],
            Address = s.Attributes.GetValueOrDefault("address", string.Empty),
            Comment = s.Attributes.GetValueOrDefault("comment", string.Empty),
            MacAddress = s.Attributes.GetValueOrDefault("mac-address", string.Empty),
            ClientId = s.Attributes.GetValueOrDefault("client-id", string.Empty),
            AddressLists = s.Attributes.GetValueOrDefault("address-lists", string.Empty),
            Server = s.Attributes.GetValueOrDefault("server", string.Empty),
            DhcpOption = s.Attributes.GetValueOrDefault("dhcp-option", string.Empty),
            Status = s.Attributes.GetValueOrDefault("status", string.Empty),
            ExpiresAfter = s.Attributes.GetValueOrDefault("expires-after", string.Empty),
            LastSeen = s.Attributes.GetValueOrDefault("last-seen", string.Empty),
            Age = s.Attributes.GetValueOrDefault("age", string.Empty),
            ActiveAddress = s.Attributes.GetValueOrDefault("active-address", string.Empty),
            ActiveMacAddress = s.Attributes.GetValueOrDefault("active-mac-address", string.Empty),
            ActiveClientId = s.Attributes.GetValueOrDefault("active-client-id", string.Empty),
            ActiveServer = s.Attributes.GetValueOrDefault("active-server", string.Empty),
            HostName = s.Attributes.GetValueOrDefault("host-name", string.Empty),
            ClassId = s.Attributes.GetValueOrDefault("class-id", string.Empty),
            Radius = s.Attributes.GetValueOrDefault("radius", "false"),
            Dynamic = s.Attributes.GetValueOrDefault("dynamic", "false"),
            Blocked = s.Attributes.GetValueOrDefault("blocked", "false"),
            Disabled = s.Attributes.GetValueOrDefault("disabled", "false")
        };
    }
}
