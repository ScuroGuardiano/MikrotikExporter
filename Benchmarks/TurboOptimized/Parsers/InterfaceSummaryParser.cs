using System.Text;
using Benchmarks.Dto;

namespace Benchmarks.TurboOptimized.Parsers;

using ParsePropAction = Action<
    InterfaceSummary, // instance of interface summary
    ReadOnlyMemory<byte> // data
>;


internal static class InterfaceSummaryParser
{
    public static IReadOnlyDictionary<ReadOnlyMemory<byte>, ParsePropAction> ParseDict { get; }
        = new Dictionary<ReadOnlyMemory<byte>, ParsePropAction>(new MemoryByteComparer())
        {
            { ".id"u8.ToArray(), (i, b) => i.Id = Encoding.UTF8.GetString(b.Span) },
            { "name"u8.ToArray(), (i, b) => i.Name = Encoding.UTF8.GetString(b.Span) },
            { "default-name"u8.ToArray(), (i, b) => i.DefaultName = Encoding.UTF8.GetString(b.Span) },
            { "actual-mtu"u8.ToArray(), (i, b) => i.ActualMtu = Encoding.UTF8.GetString(b.Span) },
            { "comment"u8.ToArray(), (i, b) => i.Comment = Encoding.UTF8.GetString(b.Span) },
            { "disabled"u8.ToArray(), (i, b) => i.Disabled = Encoding.UTF8.GetString(b.Span) },
            { "fp-rx-byte"u8.ToArray(), (i, b) => i.FpRxByte = Encoding.UTF8.GetString(b.Span) },
            { "fp-rx-packet"u8.ToArray(), (i, b) => i.FpRxPacket = Encoding.UTF8.GetString(b.Span) },
            { "fp-tx-byte"u8.ToArray(), (i, b) => i.FpTxByte = Encoding.UTF8.GetString(b.Span) },
            { "fp-tx-packet"u8.ToArray(), (i, b) => i.FpTxPacket = Encoding.UTF8.GetString(b.Span) },
            { "l2mtu"u8.ToArray(), (i, b) => i.L2Mtu = Encoding.UTF8.GetString(b.Span) },
            { "last-link-up-time"u8.ToArray(), (i, b) => i.LastLinkUpTime = Encoding.UTF8.GetString(b.Span) },
            { "last-link-down-time"u8.ToArray(), (i, b) => i.LastLinkDownTime = Encoding.UTF8.GetString(b.Span) },
            { "link-downs"u8.ToArray(), (i, b) => i.LinkDowns = Encoding.UTF8.GetString(b.Span) },
            { "mac-address"u8.ToArray(), (i, b) => i.MacAddress = Encoding.UTF8.GetString(b.Span) },
            { "mtu"u8.ToArray(), (i, b) => i.Mtu = Encoding.UTF8.GetString(b.Span) },
            { "running"u8.ToArray(), (i, b) => i.Running = Encoding.UTF8.GetString(b.Span) },
            { "rx-byte"u8.ToArray(), (i, b) => i.RxByte = Encoding.UTF8.GetString(b.Span) },
            { "rx-drop"u8.ToArray(), (i, b) => i.RxDrop = Encoding.UTF8.GetString(b.Span) },
            { "rx-error"u8.ToArray(), (i, b) => i.RxError = Encoding.UTF8.GetString(b.Span) },
            { "rx-packet"u8.ToArray(), (i, b) => i.RxPacket = Encoding.UTF8.GetString(b.Span) },
            { "tx-byte"u8.ToArray(), (i, b) => i.TxByte = Encoding.UTF8.GetString(b.Span) },
            { "tx-drop"u8.ToArray(), (i, b) => i.TxDrop = Encoding.UTF8.GetString(b.Span) },
            { "tx-error"u8.ToArray(), (i, b) => i.TxError = Encoding.UTF8.GetString(b.Span) },
            { "tx-packet"u8.ToArray(), (i, b) => i.TxPacket = Encoding.UTF8.GetString(b.Span) },
            { "tx-queue-drop"u8.ToArray(), (i, b) => i.TxQueueDrop = Encoding.UTF8.GetString(b.Span) },
            { "type"u8.ToArray(), (i, b) => i.Type = Encoding.UTF8.GetString(b.Span) }
        };
    
    public static InterfaceSummary ToInterfaceSummary(this MikrotikSentence s)
    {
        var interfaceSummary = new InterfaceSummary();
        var sentenceReader = new MikrotikSentenceReader(s);

        foreach (var kvp in sentenceReader.Iterate())
        {
            ParseDict.GetValueOrDefault(kvp.Key)?.Invoke(interfaceSummary, kvp.Value);
        }
        
        return interfaceSummary;
    }
}
