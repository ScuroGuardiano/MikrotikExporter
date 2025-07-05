using System.Text.Json.Serialization;

namespace MikrotikApiClient.Dto;

public class InterfaceSummary
{
    [JsonPropertyName(".id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("actual-mtu")]
    public string ActualMtu { get; set; } = string.Empty;

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonPropertyName("disabled")]
    public string Disabled { get; set; } = string.Empty;

    [JsonPropertyName("fp-rx-byte")]
    public string FpRxByte { get; set; } = "0";

    [JsonPropertyName("fp-rx-packet")]
    public string FpRxPacket { get; set; } = "0";

    [JsonPropertyName("fp-tx-byte")]
    public string FpTxByte { get; set; } = "0";

    [JsonPropertyName("fp-tx-packet")]
    public string FpTxPacket { get; set; } = "0";

    [JsonPropertyName("l2mtu")]
    public string L2Mtu { get; set; } = string.Empty;

    [JsonPropertyName("last-link-up-time")]
    public string LastLinkUpTime { get; set; } = string.Empty;
    
    [JsonPropertyName("last-link-down-time")]
    public string LastLinkDownTime { get; set; } = string.Empty;

    [JsonPropertyName("link-downs")]
    public string LinkDowns { get; set; } = string.Empty;

    [JsonPropertyName("mac-address")]
    public string MacAddress { get; set; } = string.Empty;

    [JsonPropertyName("mtu")]
    public string Mtu { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("default-name")]
    public string DefaultName { get; set; } = string.Empty;

    [JsonPropertyName("running")]
    public string Running { get; set; } = string.Empty;

    [JsonPropertyName("rx-byte")]
    public string RxByte { get; set; } = "0";

    [JsonPropertyName("rx-drop")]
    public string RxDrop { get; set; } = "0";

    [JsonPropertyName("rx-error")]
    public string RxError { get; set; } = "0";

    [JsonPropertyName("rx-packet")]
    public string RxPacket { get; set; } = "0";

    [JsonPropertyName("tx-byte")]
    public string TxByte { get; set; } = "0";

    [JsonPropertyName("tx-drop")]
    public string TxDrop { get; set; } = "0";

    [JsonPropertyName("tx-error")]
    public string TxError { get; set; } = "0";

    [JsonPropertyName("tx-packet")]
    public string TxPacket { get; set; } = "0";

    [JsonPropertyName("tx-queue-drop")]
    public string TxQueueDrop { get; set; } = "0";

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
