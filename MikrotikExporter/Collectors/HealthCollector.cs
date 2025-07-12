using MikrotikApiClient;
using MikrotikApiClient.Dto;

namespace MikrotikExporter.Collectors;

public class HealthCollector
{
    private readonly IMikrotikConcurrentApiClient _client;

    public HealthCollector(IMikrotikConcurrentApiClient client)
    {
        _client = client;
    }

    public async Task<MetricsCollection> Collect(bool enabled = true)
    {
        if (!enabled)
        {
            return MetricsCollection.Empty;
        }

        var healthRecords = await _client.GetHealthStats();
        
        // Alright, this will be kinda different because, frankly, I don't know how much stats can be exported here.
        // So I will create metrics dynamically.


        Dictionary<string, string> staticLabels = new()
        {
            ["router"] = _client.Name,
            ["router_host"] = _client.Host,
        };

        // Alright, funny thing. Basically I need to create dynamic list of metrics for each record
        // But my stupid little prometheus library doesn't support that.
        // So I need to create separate metrics collection for EACH value. Ugh... Anyways
        // At least I added merge to merge 'em together!!!

        List<MetricsCollection<HealthStat>> collections = [];

        foreach (var healthRecord in healthRecords)
        {
            var g = new Gauge<HealthStat>(
                $"mikrotik_health_{healthRecord.Name.ToLowerInvariant()}",
                $"Mikrotik health {healthRecord.Name} value with an unit of {healthRecord.Type}",
                h => h.Value
            );

            var m = new MetricsCollection<HealthStat>();
            m.CreateValueSet(g, staticLabels);
            collections.Add(m);

            Dictionary<string, string> labels = new()
            {
                ["type"] = healthRecord.Type,
                ["name"] = healthRecord.Name,
                ["id"] = healthRecord.Id
            };
            
            m.AddValue(healthRecord, labels);
        }
        
        return MetricsCollection<HealthStat>.Merge(collections);
    }
}
