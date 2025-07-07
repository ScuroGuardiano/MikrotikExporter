using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class IpPoolParser
{
    public static IpPool ToIpPool(this MikrotikSentence s)
    {
        return new IpPool
        {
            Id = s.Attributes[".id"],
            Name = s.Attributes["name"],
            Comment = s.Attributes.GetValueOrDefault("comment", string.Empty),
            
            Ranges = s.Attributes.GetValueOrDefault("ranges", string.Empty),
            Total = s.Attributes.GetValueOrDefault("total", "0"),
            Available = s.Attributes.GetValueOrDefault("available", "0"),
            Used = s.Attributes.GetValueOrDefault("used", "0"),
        };
    }
}
