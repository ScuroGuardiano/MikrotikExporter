using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class IpFirewallConnectionParser
{
    public static IpFirewallConnection ToIpFirewallConnection(this MikrotikSentence s)
    {
        return new IpFirewallConnection
        {
            Id = s.Attributes[".id"],
            Protocol = s.Attributes.GetValueOrDefault("protocol", string.Empty),
            SrcAddress = s.Attributes.GetValueOrDefault("src-address", string.Empty),
            DstAddress = s.Attributes.GetValueOrDefault("dst-address", string.Empty),
            ReplySrcAddress = s.Attributes.GetValueOrDefault("reply-src-address", string.Empty),
            ReplyDstAddress = s.Attributes.GetValueOrDefault("reply-dst-address", string.Empty),
            Timeout = s.Attributes.GetValueOrDefault("timeout", string.Empty),

            OrigPackets = s.Attributes.GetValueOrDefault("orig-packets", "0"),
            OrigBytes = s.Attributes.GetValueOrDefault("orig-bytes", "0"),
            OrigFasttrackPackets = s.Attributes.GetValueOrDefault("orig-fasttrack-packets", "0"),
            OrigFasttrackBytes = s.Attributes.GetValueOrDefault("orig-fasttrack-bytes", "0"),

            ReplPackets = s.Attributes.GetValueOrDefault("repl-packets", "0"),
            ReplBytes = s.Attributes.GetValueOrDefault("repl-bytes", "0"),
            ReplFasttrackPackets = s.Attributes.GetValueOrDefault("repl-fasttrack-packets", "0"),
            ReplFasttrackBytes = s.Attributes.GetValueOrDefault("repl-fasttrack-bytes", "0"),

            OrigRate = s.Attributes.GetValueOrDefault("orig-rate", "0"),
            ReplRate = s.Attributes.GetValueOrDefault("repl-rate", "0"),

            Expected = s.Attributes.GetValueOrDefault("expected", "false"),
            SeenReply = s.Attributes.GetValueOrDefault("seen-reply", "false"),
            Assured = s.Attributes.GetValueOrDefault("assured", "false"),
            Confirmed = s.Attributes.GetValueOrDefault("confirmed", "false"),
            Dying = s.Attributes.GetValueOrDefault("dying", "false"),
            Fasttrack = s.Attributes.GetValueOrDefault("fasttrack", "false"),
            HwOffload = s.Attributes.GetValueOrDefault("hw-offload", "false"),
            SrcNat = s.Attributes.GetValueOrDefault("srcnat", "false"),
            DstNat = s.Attributes.GetValueOrDefault("dstnat", "false"),
        };
    }
}
