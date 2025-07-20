using System.Text;
using Benchmarks.Dto;

namespace Benchmarks.Original.Parsers;

using ParsePropAction = Action<
    InterfaceSummary, // instance of interface summary
    ReadOnlyMemory<byte> // data
>;

internal static class InterfaceSummaryParser
{
    public static IReadOnlyDictionary<string, ParsePropAction> ParseDict { get; }
        = new Dictionary<string, ParsePropAction>
        {
            [".id"] = (i, b) => i.Id = Encoding.UTF8.GetString(b.Span),
            ["name"] = (i, b) => i.Name = Encoding.UTF8.GetString(b.Span),
            ["default-name"] = (i, b) => i.DefaultName = Encoding.UTF8.GetString(b.Span),
            ["actual-mtu"] = (i, b) => i.ActualMtu = Encoding.UTF8.GetString(b.Span),
            ["comment"] = (i, b) => i.Comment = Encoding.UTF8.GetString(b.Span),
            ["disabled"] = (i, b) => i.Disabled = Encoding.UTF8.GetString(b.Span),
            ["fp-rx-byte"] = (i, b) => i.FpRxByte = Encoding.UTF8.GetString(b.Span),
            ["fp-rx-packet"] = (i, b) => i.FpRxPacket = Encoding.UTF8.GetString(b.Span),
            ["fp-tx-byte"] = (i, b) => i.FpTxByte = Encoding.UTF8.GetString(b.Span),
            ["fp-tx-packet"] = (i, b) => i.FpTxPacket = Encoding.UTF8.GetString(b.Span),
            ["l2mtu"] = (i, b) => i.L2Mtu = Encoding.UTF8.GetString(b.Span),
            ["last-link-up-time"] = (i, b) => i.LastLinkUpTime = Encoding.UTF8.GetString(b.Span),
            ["last-link-down-time"] = (i, b) => i.LastLinkDownTime = Encoding.UTF8.GetString(b.Span),
            ["link-downs"] = (i, b) => i.LinkDowns = Encoding.UTF8.GetString(b.Span),
            ["mac-address"] = (i, b) => i.MacAddress = Encoding.UTF8.GetString(b.Span),
            ["mtu"] = (i, b) => i.Mtu = Encoding.UTF8.GetString(b.Span),
            ["running"] = (i, b) => i.Running = Encoding.UTF8.GetString(b.Span),
            ["rx-byte"] = (i, b) => i.RxByte = Encoding.UTF8.GetString(b.Span),
            ["rx-drop"] = (i, b) => i.RxDrop = Encoding.UTF8.GetString(b.Span),
            ["rx-error"] = (i, b) => i.RxError = Encoding.UTF8.GetString(b.Span),
            ["rx-packet"] = (i, b) => i.RxPacket = Encoding.UTF8.GetString(b.Span),
            ["tx-byte"] = (i, b) => i.TxByte = Encoding.UTF8.GetString(b.Span),
            ["tx-drop"] = (i, b) => i.TxDrop = Encoding.UTF8.GetString(b.Span),
            ["tx-error"] = (i, b) => i.TxError = Encoding.UTF8.GetString(b.Span),
            ["tx-packet"] = (i, b) => i.TxPacket = Encoding.UTF8.GetString(b.Span),
            ["tx-queue-drop"] = (i, b) => i.TxQueueDrop = Encoding.UTF8.GetString(b.Span),
            ["type"] = (i, b) => i.Type = Encoding.UTF8.GetString(b.Span)
        };
    
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