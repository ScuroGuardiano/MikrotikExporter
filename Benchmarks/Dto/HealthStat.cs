namespace Benchmarks.Dto;

public class HealthStat
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public string Type { get; set; } = string.Empty;
}
