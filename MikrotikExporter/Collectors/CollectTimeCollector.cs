using System.Diagnostics;
using System.Globalization;
using MikrotikApiClient;

namespace MikrotikExporter.Collectors;

public class CollectTimeCollector : BaseCollector
{
    private readonly IMikrotikApiClient _client;
    private long _startTimestamp;

    private static readonly Gauge<TimeSpan> ExportCollectTime = new(
        "mikrotik_exporter_collect_time",
        "Time it took to collect this metrics",
        ts => ts.TotalSeconds.ToString(CultureInfo.InvariantCulture)
    );

    public CollectTimeCollector(IMikrotikApiClient client)
    {
        _client = client;
    }

    public void Start()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
    }

    public MetricsCollection Collect()
    {
        var elapsed = Stopwatch.GetElapsedTime(_startTimestamp);
        
        MetricsCollection<TimeSpan> collectTimeMetric = new();
        collectTimeMetric.CreateValueSet(ExportCollectTime);
        collectTimeMetric.AddValue(elapsed);
        
        return collectTimeMetric;
    }
}