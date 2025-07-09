namespace MikrotikApiClient.Dto;

public class DnsCacheRecord
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Data { get; set; }
    public required string Name { get; set; }
    public string Ttl { get; set; } = string.Empty;
    public string Static { get; set; } = "false";
}
