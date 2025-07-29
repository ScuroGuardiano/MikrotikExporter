using System.Buffers;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MikrotikApiClient.Tcp;

internal sealed class MikrotikTcpApiConnection : IDisposable
{
    private readonly string _host;
    private readonly string _username;
    private readonly string _password;
    private readonly ILogger? _logger;

    private readonly byte[] _readLenBuffer = new byte[4];
    private readonly byte[] _writeLenBuffer = new byte[5];
    private bool _running = false;
    private readonly MemoryStream _bufferStream = new(256 * 1024);

    private readonly TcpClient _client;
    
    public MikrotikTcpApiConnection(string host, string username, string password, ILogger? logger = null)
    {
        _host = host;
        _username = username;
        _password = password;
        _logger = logger;
        _client = new TcpClient();
        _client.SendTimeout = 10000;
        _client.ReceiveTimeout = 10000;
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
            _logger?.LogInformation("Starting TCP connection to Mikrotik");
            await OpenSocket(cancellationToken);
            _logger?.LogInformation(
                "TCP connection opened {Src} -> {Dst}",
                _client.Client.LocalEndPoint?.Serialize().ToString(),
                _client.Client.RemoteEndPoint?.Serialize().ToString()
            );
        }
        
        await Login(cancellationToken);

        _running = true;
    }

    private async Task Login(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Authenticating TCP connection to Mikrotik");
        
        var res = await Request(["/login", $"=name={_username}", $"=password={_password}"]);
        res.EnsureSuccess(_logger);
        
        _logger?.LogInformation("Authenticated TCP connection to Mikrotik");
    }
    
    /// <summary>
    /// This method has no cancellation token because writing and reading can't be stopped.
    /// Otherwise the state of connection may remain invalid, that's the limitation of long-living connections.
    /// </summary>
    /// <param name="sentence"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<MikrotikResponse> Request(string[] sentence)
    {
        if (sentence.Length == 0)
        {
            throw new Exception("Sentence is empty");
        }
        
        await WriteSentence(sentence);
        await CopyResponseToBuffer();
        
        List<MikrotikSentence> sentences = [];

        for (var resSentence = ReadSentence();
             !resSentence.IsDone;
             resSentence = ReadSentence())
        {
            sentences.Add(resSentence);
        }

        return new MikrotikResponse
        {
            Sentences = sentences,
            ContainsErrors = sentences.Any(s => s.IsError),
            RequestSentence = sentence,
        };
    }


    // Each Mikrotik API response stream ends with `!done\0`
    // So that's how I know it's the end of stream
    private static readonly ReadOnlyMemory<byte> ResponseEnd = "!done\0"u8.ToArray();
    private readonly byte[] _responseEndBuffer = new byte[ResponseEnd.Length];
    private async Task CopyResponseToBuffer()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        
        var netstream = _client.GetStream();
        _bufferStream.Seek(0, SeekOrigin.Begin);
        
        try
        {
            while (true)
            {
                var read = await netstream.ReadAsync(buffer);
                _bufferStream.Write(buffer, 0, read);

                if (_bufferStream.Position < ResponseEnd.Length)
                {
                    continue;
                }
                
                // Check for response end.
                _bufferStream.Seek(-ResponseEnd.Length, SeekOrigin.Current);
                _bufferStream.ReadExactly(_responseEndBuffer);
                
                if (_responseEndBuffer.AsSpan().SequenceEqual(ResponseEnd.Span))
                {
                    // end of stream
                    _bufferStream.Seek(0, SeekOrigin.Begin);
                    return;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
    
    private async Task WriteSentence(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            await WriteWord(word);
        }

        // Every sentence ends with an empty word
        await WriteWord("");
    }
    
    private async Task WriteWord(string word)
    {
        await WriteLength((uint)word.Length);
        await WriteString(word);
    }

    private MikrotikSentence ReadSentence()
    {
        // First word is reply
        var reply = ReadWord();
        string? tag = null;
        Dictionary<string, string> attributes = new();
        
        for (ReadOnlySpan<char> word = ReadWord(); word != string.Empty; word = ReadWord())
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

    private string ReadWord()
    {
        var len = (int) ReadLength();

        if (len == 0)
        {
            return string.Empty;
        }
        
        var buffer = ArrayPool<byte>.Shared.Rent(len);
        
        try
        {
            _bufferStream.ReadExactly(buffer, 0, len);
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

    private void CloseSocket()
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

    private uint ReadLength()
    {
        _bufferStream.ReadExactly(_readLenBuffer, 0, 1);
        
        uint c = _readLenBuffer[0];
        
        if ((c & 0x80) == 0x00)
        {
            return c;
        }

        if ((c & 0xC0) == 0x80)
        {
            _bufferStream.ReadExactly(_readLenBuffer, 0, 1);
            
            const uint x = 0xC0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
        }
        else if ((c & 0xE0) == 0xC0)
        {
            _bufferStream.ReadExactly(_readLenBuffer, 0, 2);
            
            const uint x = 0xE0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
        }
        else if ((c & 0xF0) == 0xE0)
        {
            _bufferStream.ReadExactly(_readLenBuffer, 0, 3);
            
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
            _bufferStream.ReadExactly(_readLenBuffer, 0, 4);
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
        _bufferStream.Dispose();
    }
}
