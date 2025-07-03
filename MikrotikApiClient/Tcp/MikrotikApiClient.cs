using System.Diagnostics;
using Microsoft.Extensions.Options;
using MikrotikApiClient.Dto;
using MikrotikApiClient.Tcp.Parsers;

namespace MikrotikApiClient.Tcp;

public sealed class MikrotikApiClient : IMikrotikApiClient, IDisposable
{
    private readonly MikrotikApiConnection _connection;
    private readonly MikrotikApiClientOptions _options;
    
    public string Host => _options.Host;
    public string Name => _options.Name ?? Host;

    public MikrotikApiClient(IOptions<MikrotikApiClientOptions> options)
    {
        _options = options.Value;
        _connection = new MikrotikApiConnection(_options.Host, _options.Username, _options.Password);
    }
    
    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        var x = Stopwatch.GetTimestamp();
        await _connection.EnsureRunning(cancellationToken);
        Console.WriteLine(Stopwatch.GetElapsedTime(x, Stopwatch.GetTimestamp()).Milliseconds);
        
        x = Stopwatch.GetTimestamp();
        
        var res = await _connection.Request(["/interface/print"], cancellationToken);
        res.EnsureSuccess();
        Console.WriteLine(Stopwatch.GetElapsedTime(x, Stopwatch.GetTimestamp()).Milliseconds);
        
        return res.Sentences
            .Where(s => s.Reply == "!re")
            .Select(s => s.ToInterfaceSummary())
            .ToArray();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}