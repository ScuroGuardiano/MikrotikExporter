using System.Buffers;
using System.Net.Sockets;
using System.Text;

namespace MikrotikApiClient.Tcp;

public sealed class MikrotikApiConnection : IDisposable
{
    private readonly string _host;
    private readonly string _username;
    private readonly string _password;
    
    private readonly byte[] _readLenBuffer = new byte[4];
    private readonly byte[] _writeLenBuffer = new byte[5];
    private bool _running = false;

    private readonly TcpClient _client;
    
    public MikrotikApiConnection(string host, string username, string password)
    {
        _host = host;
        _username = username;
        _password = password;
        _client = new TcpClient();
    }


    public async Task EnsureRunning(CancellationToken cancellationToken = default)
    {
        if (!_running)
        {
            await Start(cancellationToken);
        }
    }

    private async Task Start(CancellationToken cancellationToken = default)
    {
        if (!_client.Connected)
        {
            await OpenSocket(cancellationToken);
        }
        
        await Login(cancellationToken);

        _running = true;
    }

    private async Task Login(CancellationToken cancellationToken = default)
    {
        var res = await Request(["/login", $"=name={_username}", $"=password={_password}"], cancellationToken);
        res.EnsureSuccess();
    }
    
    public async Task<MikrotikResponse> Request(IEnumerable<string> sentence, CancellationToken cancellationToken = default)
    {
        await WriteSentence(sentence, cancellationToken);
        List<MikrotikSentence> sentences = [];

        for (var resSentence = await ReadSentence();
             !resSentence.IsDone;
             resSentence = await ReadSentence())
        {
            sentences.Add(resSentence);
        }

        return new MikrotikResponse
        {
            Sentences = sentences,
            ContainsErrors = sentences.Any(s => s.IsError),
        };
    }

    private async Task WriteSentence(IEnumerable<string> words, CancellationToken cancellationToken)
    {
        foreach (var word in words)
        {
            await WriteWord(word, cancellationToken);
        }

        // Every sentence ends with an empty word
        await WriteWord("", cancellationToken);
    }
    
    private async Task WriteWord(string word, CancellationToken cancellationToken = default)
    {
        await WriteLength((uint)word.Length, cancellationToken);
        await WriteString(word, cancellationToken);
    }

    private async Task<MikrotikSentence> ReadSentence()
    {
        // First word is reply
        var reply = await ReadWord();
        string? tag = null;
        Dictionary<string, string> attributes = new();
        
        for (ReadOnlySpan<char> word = await ReadWord(); word != string.Empty; word = await ReadWord())
        {
            if (word.StartsWith(".tag"))
            {
                tag = word[(word.IndexOf('=') + 1)..].ToString();
            }

            if (word.StartsWith('='))
            {
                word = word[1..];
                var x = word.IndexOf('=');
                attributes[word[..x].ToString()] = word[(x + 1)..].ToString();
            }
        }

        return new MikrotikSentence
        {
            Reply = reply,
            Tag = tag,
            Attributes = attributes
        };
    }

    private async Task<string> ReadWord(CancellationToken cancellationToken = default)
    {
        var len = (int) await ReadLength(cancellationToken);

        if (len == 0)
        {
            return string.Empty;
        }
        
        var buffer = ArrayPool<byte>.Shared.Rent(len);
        
        try
        {
            await _client.GetStream().ReadExactlyAsync(buffer, 0, len, cancellationToken);
            return Encoding.UTF8.GetString(buffer, 0, len);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async Task OpenSocket(CancellationToken cancellationToken = default)
    {
        var hp = _host.Split(":");
        var host = hp[0];
        var port = hp.Length > 1 ? int.Parse(hp[1]) : 8728;

        await _client.ConnectAsync(host, port, cancellationToken);
    }

    private void CloseSocket(CancellationToken cancellationToken = default)
    {
        _client.Close();
    }
    
    private async Task WriteBytes(byte[] bytes, CancellationToken cancellationToken = default)
    {
        await _client.GetStream().WriteAsync(bytes, cancellationToken);
    }
    
    private async Task WriteBytes(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        await _client.GetStream().WriteAsync(bytes, cancellationToken);
    }

    private async Task<uint> ReadLength(CancellationToken cancellationToken = default)
    {
        await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 1, cancellationToken);
        
        uint c = _readLenBuffer[0];
        
        if ((c & 0x80) == 0x00)
        {
            return c;
        }

        if ((c & 0xC0) == 0x80)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 1, cancellationToken);
            
            const uint x = 0xC0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
        }
        else if ((c & 0xE0) == 0xC0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 2, cancellationToken);
            
            const uint x = 0xE0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
        }
        else if ((c & 0xF0) == 0xE0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 3, cancellationToken);
            
            const uint x = 0xF0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
            c <<= 8;
            c += _readLenBuffer[2];
        }
        else if ((c & 0xF8) == 0xF0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 4, cancellationToken);
            c = _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
            c <<= 8;
            c += _readLenBuffer[2];
            c <<= 8;
            c += _readLenBuffer[3];
        }

        return c;
    }

    /// <summary>
    /// Length encoding according to Mikrotik API docs
    /// </summary>
    /// <param name="length"></param>
    /// <param name="cancellationToken"></param>
    private async Task WriteLength(uint length, CancellationToken cancellationToken = default)
    {
        switch (length)
        {
            case < 0x80:
                _writeLenBuffer[0] = (byte)(length);
                await WriteBytes(_writeLenBuffer.AsMemory(0, 1), cancellationToken);
                break;
            case < 0x4000:
            {
                length |= 0x8000;
                uint b1 = (length >> 8) & 0xFF;
                uint b2 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 2), cancellationToken);
                break;
            }
            case < 0x200000:
            {
                length |= 0xC00000;
                uint b1 = (length >> 16) & 0xFF;
                uint b2 = (length >> 8) & 0xFF;
                uint b3 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 3), cancellationToken);
                break;
            }
            case < 0x10000000:
            {
                length |= 0xE0000000;
                uint b1 = (length >> 24) & 0xFF;
                uint b2 = (length >> 16) & 0xFF;
                uint b3 = (length >> 8) & 0xFF;
                uint b4 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
                _writeLenBuffer[3] = (byte)(b4);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 4), cancellationToken);
                break;
            }
            default:
            {
                const byte b1 = 0xF0;
                uint b2 = (length >> 24) & 0xFF;
                uint b3 = (length >> 16) & 0xFF;
                uint b4 = (length >> 8) & 0xFF;
                uint b5 = length & 0xFF;

                _writeLenBuffer[0] = b1;
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
                _writeLenBuffer[3] = (byte)(b4);
                _writeLenBuffer[4] = (byte)(b5);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 5), cancellationToken);
                break;
            }
        }
    }
    
    private async Task WriteString(string s, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        await WriteBytes(bytes, cancellationToken);
    }



    public void Dispose()
    {
        _client.Dispose();
    }
}
