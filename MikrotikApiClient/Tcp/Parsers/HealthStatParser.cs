using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class HealthStatParser
{
    public static HealthStat ToHealthStat(this MikrotikSentence s)
    {
        return new HealthStat
        {
            Id = s.Attributes[".id"],
            Name = s.Attributes["name"],
            Value = s.Attributes["value"],
            Type = s.Attributes.GetValueOrDefault("type", string.Empty)
        };
    }
}
