using System.ComponentModel.DataAnnotations;

namespace MikrotikApiClient;

public class MikrotikApiClientOptions
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
    
    [Required]
    public required string Host { get; set; }
    public string? Name { get; set; }

    [Range(1, MaxConnectionPoolSize)]
    public int ConnectionPool { get; set; } = DefaultConnectionPoolSize;
    
    public const int DefaultConnectionPoolSize = 4;
    
    // I can't imagine a justification for larger connection pool
    // Like really, are you trying to DoS your router to death or something?
    public const int MaxConnectionPoolSize = 32;
}
