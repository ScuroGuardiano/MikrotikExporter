using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class InterfaceSummaryParser
{
    public static InterfaceSummary ToInterfaceSummary(this MikrotikSentence s)
    {
        return new InterfaceSummary
        {
            Id = s.Attributes[".id"],
            Name = s.Attributes["name"],
            DefaultName = s.Attributes.GetValueOrDefault("default-name", ""),
            ActualMtu = s.Attributes.GetValueOrDefault("actual-mtu", ""),
            Comment = s.Attributes.GetValueOrDefault("comment", ""),
            Disabled = s.Attributes.GetValueOrDefault("disabled", ""),
            FpRxByte = s.Attributes.GetValueOrDefault("fp-rx-byte", "0"),
            FpRxPacket = s.Attributes.GetValueOrDefault("fp-rx-packet", "0"),
            FpTxByte = s.Attributes.GetValueOrDefault("fp-tx-byte", "0"),
            FpTxPacket = s.Attributes.GetValueOrDefault("fp-tx-packet", "0"),
            L2Mtu = s.Attributes.GetValueOrDefault("l2mtu", ""),
            LastLinkUpTime = s.Attributes.GetValueOrDefault("last-link-up-time", ""),
            LastLinkDownTime = s.Attributes.GetValueOrDefault("last-link-down-time", ""),
            LinkDowns = s.Attributes.GetValueOrDefault("link-downs", ""),
            MacAddress = s.Attributes.GetValueOrDefault("mac-address", ""),
            Mtu = s.Attributes.GetValueOrDefault("mtu", ""),
            Running = s.Attributes.GetValueOrDefault("running", ""),
            RxByte = s.Attributes.GetValueOrDefault("rx-byte", "0"),
            RxDrop = s.Attributes.GetValueOrDefault("rx-drop", "0"),
            RxError = s.Attributes.GetValueOrDefault("rx-error", "0"),
            RxPacket = s.Attributes.GetValueOrDefault("rx-packet", "0"),
            TxByte = s.Attributes.GetValueOrDefault("tx-byte", "0"),
            TxDrop = s.Attributes.GetValueOrDefault("tx-drop", "0"),
            TxError = s.Attributes.GetValueOrDefault("tx-error", "0"),
            TxPacket = s.Attributes.GetValueOrDefault("tx-packet", "0"),
            TxQueueDrop = s.Attributes.GetValueOrDefault("tx-queue-drop", "0"),
            Type = s.Attributes.GetValueOrDefault("type", "")
        };
    }
}
