namespace MikrotikApiClient.Tcp;

internal class MikrotikSentence
{
    public required string Reply { get; init; }
    public string? Tag  { get; init; }
    public required IReadOnlyDictionary<string, string> Attributes { get; init; }

    public bool IsDone => Reply is "!done";
    public bool IsError => Reply is "!trap" or "!fatal";
    public bool IsFatal => Reply is "!fatal";
    public bool IsEmpty => Reply is "!empty";
}
