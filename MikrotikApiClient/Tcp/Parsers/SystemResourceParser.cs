using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class SystemResourceParser
{
    public static SystemResource ToSystemResource(this MikrotikSentence s)
    {
        return new SystemResource
        {
            Uptime = s.Attributes.GetValueOrDefault("uptime", string.Empty),
            Version = s.Attributes.GetValueOrDefault("version", string.Empty),
            BuildTime = s.Attributes.GetValueOrDefault("build-time", string.Empty),
            FactorySoftware = s.Attributes.GetValueOrDefault("factory-software", string.Empty),
            FreeMemory = s.Attributes.GetValueOrDefault("free-memory", "0"),
            TotalMemory = s.Attributes.GetValueOrDefault("total-memory", "0"),
            Cpu = s.Attributes.GetValueOrDefault("cpu", string.Empty),
            CpuCount = s.Attributes.GetValueOrDefault("cpu-count", "0"),
            CpuFrequency = s.Attributes.GetValueOrDefault("cpu-frequency", "0"),
            CpuLoad = s.Attributes.GetValueOrDefault("cpu-load", "0"),
            FreeHddSpace = s.Attributes.GetValueOrDefault("free-hdd-space", "0"),
            TotalHddSpace = s.Attributes.GetValueOrDefault("total-hdd-space", "0"),
            WriteSectSinceReboot = s.Attributes.GetValueOrDefault("write-sect-since-reboot", "0"),
            WriteSectTotal = s.Attributes.GetValueOrDefault("write-sect-total", "0"),
            BadBlocks = s.Attributes.GetValueOrDefault("bad-blocks", "0"),
            ArchitectureName = s.Attributes.GetValueOrDefault("architecture-name", string.Empty),
            BoardName = s.Attributes.GetValueOrDefault("board-name", string.Empty),
            Platform = s.Attributes.GetValueOrDefault("platform", string.Empty)
        };
    }
}
