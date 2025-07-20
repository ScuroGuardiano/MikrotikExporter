using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Benchmarks.TurboOptimized;

internal sealed class MikrotikSentence : IDisposable
{
    [SetsRequiredMembers]
    public MikrotikSentence(string reply, byte[] data, int dataLength)
    {
        Reply = reply;
        _data = data;
        _dataLength = dataLength;
    }
    
    public required string Reply { get; init; }
    public ReadOnlyMemory<byte> Data => _data.AsMemory(0, _dataLength);

    private byte[] _data; // <- this is from ArrayPool, must be returned!
    private bool _disposed;
    private readonly int _dataLength;

    public bool IsDone => Reply is "!done";
    public bool IsError => Reply is "!trap" or "!fatal";
    public bool IsFatal => Reply is "!fatal";
    public bool IsEmpty => Reply is "!empty";

    public void Dispose()
    {
        if (_disposed) return;
        GC.SuppressFinalize(this);
        
        ArrayPool<byte>.Shared.Return(_data);
        _data = null!; // Just to make sure it can't be used by an accident
        _disposed = true;
    }

    ~MikrotikSentence()
    {
        Dispose();
    }
}
