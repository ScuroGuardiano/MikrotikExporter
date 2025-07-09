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

        for (var i = 0; i < time.Length; i++)
        {
            char c = time[i];
            
            if (char.IsDigit(c))
            {
                curr += c;
            }
            else
            {
                if (c == 'm' && (time.Length > i + 1) && time[i + 1] == 's')
                {
                    i++;
                    timeSpan = TimeSpan.FromMilliseconds(int.Parse(curr));
                    continue;
                }
                
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
