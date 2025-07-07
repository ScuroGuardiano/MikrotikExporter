using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class IpFirewallRuleParser
{
    public static IpFirewallRule ToIpFirewallRule(this MikrotikSentence s, string table)
    {
        return new IpFirewallRule
        {
            Id = s.Attributes[".id"],
            Chain = s.Attributes["chain"],
            Action = s.Attributes["action"],
            Table = table,

            Comment = s.Attributes.GetValueOrDefault("comment", string.Empty),

            Bytes = s.Attributes.GetValueOrDefault("bytes", "0"),
            Packets = s.Attributes.GetValueOrDefault("packets", "0"),

            ConnectionState = s.Attributes.GetValueOrDefault("connection-state", string.Empty),
            ConnectionNatState = s.Attributes.GetValueOrDefault("connection-nat-state", string.Empty),

            InInterface = s.Attributes.GetValueOrDefault("in-interface", string.Empty),
            OutInterface = s.Attributes.GetValueOrDefault("out-interface", string.Empty),
            InInterfaceList = s.Attributes.GetValueOrDefault("in-interface-list", string.Empty),
            OutInterfaceList = s.Attributes.GetValueOrDefault("out-interface-list", string.Empty),
            SrcPort = s.Attributes.GetValueOrDefault("src-port", string.Empty),
            DstPort = s.Attributes.GetValueOrDefault("dst-port", string.Empty),
            SrcAddress = s.Attributes.GetValueOrDefault("src-address", string.Empty),
            DstAddress = s.Attributes.GetValueOrDefault("dst-address", string.Empty),
            SrcAddressList = s.Attributes.GetValueOrDefault("src-address-list", string.Empty),
            DstAddressList = s.Attributes.GetValueOrDefault("dst-address-list", string.Empty),

            ToAddresses = s.Attributes.GetValueOrDefault("to-addresses", string.Empty),
            ToPorts = s.Attributes.GetValueOrDefault("to-ports", string.Empty),

            Invalid = s.Attributes.GetValueOrDefault("invalid", string.Empty),
            Dynamic = s.Attributes.GetValueOrDefault("dynamic", string.Empty),
            Disabled = s.Attributes.GetValueOrDefault("disabled", string.Empty),
        };
    }
}