namespace Benchmarks.Dto;

public class SystemResource
{
    public string Uptime { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BuildTime { get; set; } = string.Empty;
    public string FactorySoftware { get; set; } = string.Empty;
    public string FreeMemory { get; set; } = "0";
    public string TotalMemory { get; set; } = "0";
    public string Cpu { get; set; } = string.Empty;
    public string CpuCount { get; set; } = "0";
    public string CpuFrequency { get; set; } = "0";
    public string CpuLoad { get; set; } = "0";
    public string FreeHddSpace { get; set; } = "0";
    public string TotalHddSpace { get; set; } = "0";
    public string WriteSectSinceReboot { get; set; } = "0";
    public string WriteSectTotal { get; set; } = "0";
    public string BadBlocks { get; set; } = "0";
    public string ArchitectureName { get; set; } = string.Empty;
    public string BoardName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
}
