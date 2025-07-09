using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class DnsCacheParser
{
    public static DnsCacheRecord ToDnsRecord(this MikrotikSentence s)
    {
        return new DnsCacheRecord
        {
            Id = s.Attributes[".id"],
            Type = s.Attributes["type"],
            Data = s.Attributes["data"],
            Name = s.Attributes["name"],
            Ttl = s.Attributes.GetValueOrDefault("ttl", string.Empty),
            Static = s.Attributes.GetValueOrDefault("static", "false")
        };
    }
}
