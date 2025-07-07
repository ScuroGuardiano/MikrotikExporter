namespace MikrotikExporter.Helpers;

public static class MikrotikTimeSpanConverter
{
    private static readonly Dictionary<char, TimeSpan> ConvertionTable = new()
    {
        ['y'] = TimeSpan.FromDays(365), // just in case, idk if Mikrotik uses it
        ['w'] = TimeSpan.FromDays(7),
        ['d'] = TimeSpan.FromDays(1),
        ['h'] = TimeSpan.FromHours(1),
        ['m'] = TimeSpan.FromMinutes(1),
        ['s'] = TimeSpan.FromSeconds(1),
    };
    
    private static readonly TimeSpan Def = TimeSpan.FromSeconds(1);
    
    public static TimeSpan ToTimeSpan(string time)
    {
        var timeSpan = TimeSpan.Zero;

        var curr = string.Empty;

        foreach (var c in time)
        {
            if (char.IsDigit(c))
            {
                curr += c;
            }
            else
            {
                timeSpan += int.Parse(curr) * ConvertionTable.GetValueOrDefault(c, Def);
                curr = string.Empty;
            }
        }
        
        return timeSpan;
    }

    public static double ToSeconds(string time)
    {
        return ToTimeSpan(time).TotalSeconds;
    }
}
