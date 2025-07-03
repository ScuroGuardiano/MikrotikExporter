namespace MikrotikExporter.PrometheusMappers;

public static class DictionaryLabelExtensions
{
    public static void AddIfNotEmpty(this Dictionary<string, string> dict, string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        
        dict.Add(key, value);
    }
}