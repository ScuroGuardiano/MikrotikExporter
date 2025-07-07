namespace MikrotikApiClient.Dto;

public class IpFirewallRule
{
    public required string Id { get; set; }
    public required string Chain { get; set; }
    public required string Action { get; set; }
    
    /// <summary>
    /// nat, mangle, raw, filter
    /// </summary>
    public required string Table { get; set; }

    public string Comment { get; set; } = string.Empty;

    public string Bytes { get; set; } = "0";
    public string Packets { get; set; } = "0";
    
    public string ConnectionState { get; set; } = string.Empty;
    public string ConnectionNatState { get; set; } = string.Empty;
    
    public string InInterface { get; set; } = string.Empty;
    public string OutInterface { get; set; } = string.Empty;
    public string InInterfaceList { get; set; } = string.Empty;
    public string OutInterfaceList { get; set; } = string.Empty;
    public string SrcPort { get; set; } =  string.Empty;
    public string DstPort { get; set; } =  string.Empty;
    public string SrcAddress { get; set; } =  string.Empty;
    public string DstAddress { get; set; } =  string.Empty;
    public string SrcAddressList { get; set; } =  string.Empty;
    public string DstAddressList { get; set; } =  string.Empty;
    
    // NAT
    public string ToAddresses { get; set; } =  string.Empty;
    public string ToPorts { get; set; } =  string.Empty;
    
    public string Invalid { get; set; } = string.Empty;
    public string Dynamic { get; set; } = string.Empty;
    public string Disabled { get; set; } = string.Empty;
}
