using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using static MultitenantStarter.Auth.Infrastructure.RedisCacheModel;

namespace MultitenantStarter.Auth.Infrastructure
{
    /// <summary>
    /// Repository class for managing Redis cache operations including storing, retrieving, and deleting cached data.
    /// </summary>
    public class RedisCacheRepository : IRedisCacheRepository, IDisposable
    {
        private readonly IDistributedCache _cache;
        private readonly RedisCacheSettings _redisCacheSettings;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private bool _disposed;

        private ConnectionMultiplexer Connection => _lazyConnection.Value;

        public RedisCacheRepository(IDistributedCache cache, IOptions<RedisCacheSettings> cacheSettings)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _redisCacheSettings = cacheSettings?.Value ?? throw new ArgumentNullException(nameof(cacheSettings));

            var connectionString = $"{_redisCacheSettings.Host}:{_redisCacheSettings.Port},password={_redisCacheSettings.Password}";
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString)));
        }

        public async Task<bool> DeleteKeysAsync(string search, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(search))
                throw new ArgumentException("Search pattern cannot be null or empty.", nameof(search));

            var keys = await FetchKeysAsync(search, cancellationToken);

            var tasks = keys.Select(key => _cache.RemoveAsync(key, cancellationToken));
            await Task.WhenAll(tasks);

            return true;
        }

        public Task<List<string>> FetchKeysAsync(string search, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(search))
                throw new ArgumentException("Search pattern cannot be null or empty.", nameof(search));

            return Task.Run(() =>
            {
                var endPoint = Connection.GetEndPoints().First();
                var pattern = $"*{search}*";
                var keys = Connection.GetServer(endPoint).Keys(pattern: pattern);

                return keys.Select(key => key.ToString()).ToList();
            }, cancellationToken);
        }

        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            return await _cache.GetStringAsync(key, cancellationToken);
        }

        public Task<List<string>> GetAllKeysAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var endPoint = Connection.GetEndPoints().First();
                var keys = Connection.GetServer(endPoint).Keys(pattern: "*");

                return keys.Select(key => key.ToString()).ToList();
            }, cancellationToken);
        }

        public async Task<bool> RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            try
            {
                await _cache.RefreshAsync(key, cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SetAsync(CacheKeyVal payload, CancellationToken cancellationToken = default)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrWhiteSpace(payload.Key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(payload.Key));

            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_redisCacheSettings.AbsoluteExpirationInSeconds),
                SlidingExpiration = TimeSpan.FromSeconds(_redisCacheSettings.SlidingExpirationInSeconds)
            };

            await _cache.SetStringAsync(payload.Key, JsonSerializer.Serialize(payload.Value), timeOut, cancellationToken);

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
            }

            _disposed = true;
        }
    }
}
