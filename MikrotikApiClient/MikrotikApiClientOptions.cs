namespace MikrotikApiClient;

public class MikrotikApiClientOptions
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
    public string? Name { get; set; }
}
