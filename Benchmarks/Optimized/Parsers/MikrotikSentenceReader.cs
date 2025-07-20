using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Benchmarks.Optimized.Parsers;

internal struct MikrotikSentenceReader
{
    private readonly MikrotikSentence _sentence;
    private int _idx;
    private State _state = State.Whatever;

    public MikrotikSentenceReader(MikrotikSentence sentence)
    {
        _sentence = sentence;
    }

    public IEnumerable<KeyValuePair<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>> Iterate()
    {
        while (ReadNext() is { } n)
        {
            yield return n;
        }
    }
    
    public KeyValuePair<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>? ReadNext()
    {
        ReadOnlySpan<byte> readin = _sentence.Data.Span[_idx..];
        if (readin.IsEmpty) return null;

        int firstEq = readin.IndexOf((byte)'=');

        if (firstEq == -1)
        {
            if (readin.Length == 1 && readin[0] == 0x00)
            {
                // it's the end of sentence
                return null;
            }
        }
        
        readin = readin.Slice(firstEq + 1);
    
        int secondEq = readin.IndexOf((byte)'=');
        if (secondEq == -1)
        {
            throw new Exception("Invalid sentence");
        }

        ReadOnlyMemory<byte> key = _sentence.Data.Slice(_idx + firstEq + 1, secondEq);
    
        readin = readin.Slice(secondEq + 1);

        int zeroIdx = readin.IndexOf((byte)0x00);
        if (zeroIdx == -1) throw new Exception("Invalid sentence");

        ReadOnlyMemory<byte> value = _sentence.Data.Slice(_idx + firstEq + secondEq + 2, zeroIdx);
        _idx += firstEq + secondEq + 1 + zeroIdx + 1;

        return new KeyValuePair<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(key, value);
    }
    
    private enum State
    {
        Whatever,
        ReadingKey,
        ReadingValue
    }
}