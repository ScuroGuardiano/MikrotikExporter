namespace MikrotikApiClient.Tcp;
internal class MikrotikResponse
{
    public required IReadOnlyList<MikrotikSentence> Sentences { get; init; }
    public bool ContainsErrors { get; init; }

    public void EnsureSuccess()
    {
        if (ContainsErrors)
        {
            // Find error sentence
            var s = Sentences.FirstOrDefault(s => s.IsError);
            
            if (s == null)
            {
                throw new Exception("Unknown error during request to Mikrotik API.");
            }
            
            throw new Exception($"Error during request to Mikrotik API: {s.Attributes.GetValueOrDefault("message")}");
        }
    }
};
