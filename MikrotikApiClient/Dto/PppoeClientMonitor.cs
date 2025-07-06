namespace MikrotikApiClient.Dto;

public class PppoeClientMonitor
{
    // There's funny thing!
    // /interface/pppoe-client/monitor DOES NOT return interface name
    // So it has to be provided from outside based on =numbers= param order XD
    // Not cool...
    public string Name { get; set; } = "to_be_provided";
    
    public string Status { get; set; } = string.Empty;
    public string Uptime { get; set; } = string.Empty;
    public string ActiveLinks { get; set; } = "0";
    public string Encoding { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string AcName { get; set; } = string.Empty;
    public string AcMac { get; set; } = string.Empty;
    public string Mtu { get; set; } = "0";
    public string Mru { get; set; } = "0";
    public string LocalAddress { get; set; } = string.Empty;
    public string RemoteAddress { get; set; } = string.Empty;
}