namespace Benchmarks.Dto;

public class WlanMonitor
{
    // There's funny thing!
    // /interface/wireless/monitor DOES NOT return interface name
    // So it has to be provided from outside based on =numbers= param order XD
    // Not cool...
    public string Name { get; set; } = "to_be_provided";
    
    public string Status { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string WirelessProtocol { get; set; } = string.Empty;
    public string NoiseFloor { get; set; } = "0";
    public string OverallTxCcq { get; set; } = "0";
    public string RegisteredClients { get; set; } = "0";
    public string AuthenticatedClients { get; set; } = "0";
    public string WmmEnabled { get; set; } = "false";
    public string NotifyExternalFdb { get; set; } = "false";
}