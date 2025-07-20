namespace Benchmarks;

public class MikrotikException : Exception
{
    internal MikrotikException(string resourcePath, string[] requestSentence, string message) : base($"Mikrotik API exception: {message}, resource: {resourcePath}")
    {
        ResourcePath = resourcePath;
        MikrotikMessage = message;
        RequestSentence = requestSentence;
    }
    
    public string ResourcePath { get; }
    public string[] RequestSentence { get; }
    public string MikrotikMessage { get; }
}
