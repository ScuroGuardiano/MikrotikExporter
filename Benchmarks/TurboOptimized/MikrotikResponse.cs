using System.Text;
using Microsoft.Extensions.Logging;

namespace Benchmarks.TurboOptimized;
internal sealed class MikrotikResponse : IDisposable
{
    public required IReadOnlyList<MikrotikSentence> Sentences { get; init; }
    public bool ContainsErrors { get; init; }
    
    public required ReadOnlyMemory<byte>[] RequestSentence { get; init; }
    public string ResourcePath => Encoding.UTF8.GetString(RequestSentence[0].Span);
    
    private bool _disposed;

    public void EnsureSuccess(ILogger? logger = null)
    {
        if (ContainsErrors)
        {

            // Find error sentence
            var s = Sentences.FirstOrDefault(s => s.IsError);
            
            // logger?.LogError(
            //     """
            //     Mikrotik response contains errors.
            //     Error Message: {ErrorMessage},
            //     Resource Path: {ResourcePath},
            //     RequestSentence:
            //     {RequestSentence}
            //     """,
            //     s?.Attributes?.GetValueOrDefault("message") ?? "Unknown",
            //     ResourcePath,
            //     string.Join('\n', RequestSentence)
            // );
            
            if (s == null)
            {
                throw new MikrotikException(ResourcePath, RequestSentence.Select(x => Encoding.UTF8.GetString(x.Span)).ToArray(), "Unknown error during request to Mikrotik API.");
            }
            
            // for now ignore details XD
            // throw new MikrotikException(ResourcePath, RequestSentence, s.Attributes?.GetValueOrDefault("message") ?? "Unknown error during request to Mikrotik API.");
            throw new MikrotikException(ResourcePath, RequestSentence.Select(x => Encoding.UTF8.GetString(x.Span)).ToArray(), "Unknown error during request to Mikrotik API.");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        GC.SuppressFinalize(this);
        foreach (var sentence in Sentences)
        {
            sentence.Dispose();
        }
        _disposed = true;
    }

    ~MikrotikResponse()
    {
        Dispose();
    }
};