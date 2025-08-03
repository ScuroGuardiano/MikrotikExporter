using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class PackageParser
{
    public static Package ToPackage(this MikrotikSentence s)
    {
        return new Package
        {
            Id = s.Attributes[".id"],
            Name = s.Attributes["name"],
            Disabled = StringToBoolean(s.Attributes.GetValueOrDefault("disabled")),
        };
    }

    private static bool StringToBoolean(string? s)
    {
        return s != null && bool.Parse(s);
    }
}
