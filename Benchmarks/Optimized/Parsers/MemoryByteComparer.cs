namespace Benchmarks.Optimized.Parsers;

internal class MemoryByteComparer : IEqualityComparer<ReadOnlyMemory<byte>>
{
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) => x.Span.SequenceEqual(y.Span);
    public int GetHashCode(ReadOnlyMemory<byte> obj)
    {
        int hash = 17;
        foreach (byte b in obj.Span) hash = hash * 23 + b;
        return hash;
    }
}
