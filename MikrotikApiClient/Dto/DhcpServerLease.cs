namespace MikrotikApiClient.Dto;

public class DhcpServerLease
{
    public required string Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string AddressLists { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string DhcpOption { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ExpiresAfter { get; set; } = string.Empty;
    public string LastSeen { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string ActiveAddress { get; set; } = string.Empty;
    public string ActiveMacAddress { get; set; } = string.Empty;
    public string ActiveClientId { get; set; } = string.Empty;
    public string ActiveServer { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string Radius { get; set; } = "false";
    public string Dynamic { get; set; } = "false";
    public string Blocked { get; set; } = "false";
    public string Disabled { get; set; } = "false";
}
