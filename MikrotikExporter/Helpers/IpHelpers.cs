namespace MikrotikExporter.Helpers;

public static class IpHelpers
{
    public static bool IsPublic(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }
        
        var ipParts = ipAddress.Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToArray();;

        if (ipParts.Length != 4)
        {
            return false;
        }
        
        if (ipParts[0] == 10 ||
            (ipParts[0] == 192 && ipParts[1] == 168) ||
            (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31))) {
            return false;
        }

        return true;
    }
}
