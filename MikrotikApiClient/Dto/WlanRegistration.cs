namespace MikrotikApiClient.Dto;

public class WlanRegistration
{
    public required string Id { get; set; }
    public required string Interface { get; set; }
    public required string MacAddress { get; set; }

    public string Ap { get; set; } = "false";
    public string Wds { get; set; } = "false";
    public string Bridge { get; set; } = "false";

    public string RxRate { get; set; } = string.Empty;
    public string TxRate { get; set; } = string.Empty;

    public string TxPackets { get; set; } = "0";
    public string RxPackets { get; set; } = "0";

    public string TxBytes { get; set; } = "0";
    public string RxBytes { get; set; } = "0";

    public string TxFrames { get; set; } = "0";
    public string RxFrames { get; set; } = "0";

    public string TxFrameBytes { get; set; } = "0";
    public string RxFrameBytes { get; set; } = "0";

    public string TxHwFrames { get; set; } = "0";
    public string RxHwFrames { get; set; } = "0";

    public string TxHwFrameBytes { get; set; } = "0";
    public string RxHwFrameBytes { get; set; } = "0";

    public string TxFramesTimedOut { get; set; } = "0";

    public string Uptime { get; set; } = string.Empty;
    public string LastActivity { get; set; } = string.Empty;

    public string SignalStrength { get; set; } = string.Empty;
    public string SignalToNoise { get; set; } = string.Empty;

    public string SignalStrengthCh0 { get; set; } = string.Empty;
    public string SignalStrengthCh1 { get; set; } = string.Empty;
    public string SignalStrengthCh2 { get; set; } = string.Empty;
    public string SignalStrengthCh3 { get; set; } = string.Empty;

    public string StrengthAtRates { get; set; } = string.Empty;
    public string TxCcq { get; set; } = string.Empty;
    public string PThroughput { get; set; } = string.Empty;

    public string LastIp { get; set; } = string.Empty;
    public string Port802_1xEnabled { get; set; } = "false";

    public string AuthenticationType { get; set; } = string.Empty;
    public string Encryption { get; set; } = string.Empty;
    public string GroupEncryption { get; set; } = string.Empty;
    public string ManagementProtection { get; set; } = "false";
    public string WmmEnabled { get; set; } = "false";

    public string TxRateSet { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
