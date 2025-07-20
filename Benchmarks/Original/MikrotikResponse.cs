using Microsoft.Extensions.Logging;

namespace Benchmarks.Original;
internal sealed class MikrotikResponse
{
    public required IReadOnlyList<MikrotikSentence> Sentences { get; init; }
    public bool ContainsErrors { get; init; }
    
    public required string[] RequestSentence { get; init; }
    public string ResourcePath => RequestSentence[0];

    public void EnsureSuccess(ILogger? logger = null)
    {
        if (ContainsErrors)
        {

            // Find error sentence
            var s = Sentences.FirstOrDefault(s => s.IsError);
            
            logger?.LogError(
                """
                Mikrotik response contains errors.
                Error Message: {ErrorMessage},
                Resource Path: {ResourcePath},
                RequestSentence:
                {RequestSentence}
                """,
                s?.Attributes?.GetValueOrDefault("message") ?? "Unknown",
                ResourcePath,
                string.Join('\n', RequestSentence)
            );
            
            if (s == null)
            {
                throw new MikrotikException(ResourcePath, RequestSentence, "Unknown error during request to Mikrotik API.");
            }
            
            throw new MikrotikException(ResourcePath, RequestSentence, s.Attributes?.GetValueOrDefault("message") ?? "Unknown error during request to Mikrotik API.");
        }
    }
};
