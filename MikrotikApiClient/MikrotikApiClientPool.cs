using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

internal sealed class MikrotikApiClientPool : IMikrotikApiClient
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ConcurrentBag<IMikrotikApiClient> _clients;
    private readonly MikrotikApiClientOptions _options;
    private bool _disposed;

    public string Host => _options.Host;
    public string Name => _options.Name ?? _options.Host;
    
    public MikrotikApiClientPool(IOptions<MikrotikApiClientOptions> options, IMikrotikApiClientFactory clientFactory)
    {
        _options = options.Value;
        
        var connectionPool = GetReasonableConnectionPoolValue(_options.ConnectionPool);
        
        _semaphoreSlim = new SemaphoreSlim(connectionPool, connectionPool);
        _clients = [];

        for (var i = 0; i < connectionPool; i++)
        {
            _clients.Add(clientFactory.CreateClient());
        }
    }
    
    public async Task<InterfaceSummary[]> GetInterfaces(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetInterfaces(cancellationToken));
    }

    public async Task<EthernetMonitor[]> GetEtherMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetEtherMonitor(numbers, cancellationToken));
    }

    public async Task<WlanMonitor[]> GetWlanMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetWlanMonitor(numbers, cancellationToken));
    }

    public async Task<PppoeClientMonitor[]> GetPppoeClientMonitor(IEnumerable<string> numbers, CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetPppoeClientMonitor(numbers, cancellationToken));
    }

    public async Task<HealthStat[]> GetHealthStats(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetHealthStats(cancellationToken));
    }

    public async Task<SystemResource> GetSystemResource(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetSystemResource(cancellationToken));
    }

    public async Task<DhcpServerLease[]> GetDhcpServerLeases(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetDhcpServerLeases(cancellationToken));
    }

    public async Task<IpFirewallConnection[]> GetIpFirewallConnections(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetIpFirewallConnections(cancellationToken));
    }

    public async Task<IpFirewallRule[]> GetIpFirewallRules(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetIpFirewallRules(cancellationToken));
    }

    public async Task<IpPool[]> GetIpPools(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetIpPools(cancellationToken));
    }

    public async Task<string> GetIdentity(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetIdentity(cancellationToken));
    }

    public async Task<DnsCacheRecord[]> GetDnsCacheRecords(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetDnsCacheRecords(cancellationToken));
    }

    public async Task<WlanRegistration[]> GetWlanRegistrations(CancellationToken cancellationToken = default)
    {
        using var loan = await _clients.BorrowClient(_semaphoreSlim, cancellationToken);
        return await loan.Execute(c => c.GetWlanRegistrations(cancellationToken));
    }

    private static int GetReasonableConnectionPoolValue(int connectionPoolFromOptions)
    {
        return connectionPoolFromOptions switch
        {
            // In case of below zero just set to default value
            < 0 => MikrotikApiClientOptions.DefaultConnectionPoolSize,
            > MikrotikApiClientOptions.MaxConnectionPoolSize => MikrotikApiClientOptions.MaxConnectionPoolSize,
            _ => connectionPoolFromOptions
        };
    }
    
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _semaphoreSlim.Dispose();
        
        foreach (var client in _clients)
        {
            if (client is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

file static class ConcurrentBagExtensions
{
    [MustDisposeResource]
    internal static async Task<Loan> BorrowClient(this ConcurrentBag<IMikrotikApiClient> clients, SemaphoreSlim semaphore,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await semaphore.WaitAsync(cancellationToken);

            if (clients.TryTake(out var client))
            {
                return new Loan(client, clients, semaphore);
            }

            throw new UnreachableException("ConcurrentBag had no element in it, which is unexpected.");
        }
        catch
        {
            semaphore.Release();
            throw;
        }
    }

    internal struct Loan : IDisposable
    {
        private IMikrotikApiClient? _client;
        private readonly ConcurrentBag<IMikrotikApiClient> _clients;
        private readonly SemaphoreSlim _semaphoreSlim;

        internal Loan(IMikrotikApiClient client, ConcurrentBag<IMikrotikApiClient> clients, SemaphoreSlim semaphoreSlim)
        {
            _client = client;
            _clients = clients;
            _semaphoreSlim = semaphoreSlim;
        }

        public T Execute<T>(Func<IMikrotikApiClient, T> func)
        {
            if (_client == null)
            {
                throw new ObjectDisposedException(nameof(Loan), "Borrow client is disposed.");
            }
            
            return func(_client!);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _clients.Add(_client);
                _semaphoreSlim.Release();
                _client = null;
            }
        }
    }

}