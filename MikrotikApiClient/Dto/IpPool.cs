namespace MikrotikApiClient.Dto;

public class IpPool
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string Comment { get; set; } = string.Empty;
    
    public string Ranges { get; set; } = string.Empty;
    public string Used { get; set; } = "0";
    public string Total { get; set; } = "0";
    public string Available { get; set; } = "0";
}