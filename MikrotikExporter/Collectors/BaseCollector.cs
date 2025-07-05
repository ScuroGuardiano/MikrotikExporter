using MikrotikApiClient;

namespace MikrotikExporter.Collectors;

public abstract class BaseCollector
{
    public bool Enabled { get; set; } = true;
}