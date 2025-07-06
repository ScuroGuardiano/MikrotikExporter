namespace MikrotikApiClient.Dto;

public class IpFirewallConnection
{
    public required string Id { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public string SrcAddress { get; set; } = string.Empty;
    public string DstAddress { get; set; } = string.Empty;
    public string ReplySrcAddress { get; set; } = string.Empty;
    public string ReplyDstAddress { get; set; } = string.Empty;
    public string Timeout { get; set; } = string.Empty;

    public string OrigPackets { get; set; } = "0";
    public string OrigBytes { get; set; } = "0";
    public string OrigFasttrackPackets { get; set; } = "0";
    public string OrigFasttrackBytes { get; set; } = "0";

    public string ReplPackets { get; set; } = "0";
    public string ReplBytes { get; set; } = "0";
    public string ReplFasttrackPackets { get; set; } = "0";
    public string ReplFasttrackBytes { get; set; } = "0";

    public string OrigRate { get; set; } = "0";
    public string ReplRate { get; set; } = "0";

    public string Expected { get; set; } = "false";
    public string SeenReply { get; set; } = "false";
    public string Assured { get; set; } = "false";
    public string Confirmed { get; set; } = "false";
    public string Dying { get; set; } = "false";
    public string Fasttrack { get; set; } = "false";
    public string HwOffload { get; set; } = "false";
    public string SrcNat { get; set; } = "false";
    public string DstNat { get; set; } = "false";
}
