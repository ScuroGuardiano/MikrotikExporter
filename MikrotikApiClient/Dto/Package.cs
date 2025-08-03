namespace MikrotikApiClient.Dto;

public class Package
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool Disabled { get; set; }
}
