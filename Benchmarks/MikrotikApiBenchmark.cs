using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Dto;
using Original = Benchmarks.Original;

namespace Benchmarks;

[MemoryDiagnoser]
public class MikrotikApiBenchmark
{
    private readonly IMikrotikApiClient _originalClient = new Original.MikrotikFakeTcpApiClient();
    private readonly IMikrotikApiClient _optimizedClient = new Optimized.MikrotikFakeTcpApiClient();
    private readonly IMikrotikApiClient _turboOptimizedClient = new TurboOptimized.MikrotikFakeTcpApiClient();
    
    [Benchmark]
    public async Task<InterfaceSummary[]> GetInterfacesOriginal()
    {
        return await _originalClient.GetInterfaces();
    }

    [Benchmark]
    public async Task<InterfaceSummary[]> GetInterfacesOptimized()
    {
        return await _optimizedClient.GetInterfaces();
    }

    [Benchmark]
    public async Task<InterfaceSummary[]> GetInterfacesTurboOptimized()
    {
        return await _turboOptimizedClient.GetInterfaces();
    }
}
