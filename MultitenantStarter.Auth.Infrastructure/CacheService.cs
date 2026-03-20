namespace MultitenantStarter.Auth.Infrastructure
{
    /// <summary>
    /// Service class for Redis cache operations that provides a higher-level abstraction over the repository.
    /// </summary>
    public class RedisCacheService
    {
        private readonly IRedisCacheRepository _redisCacheRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="redisCacheRepository">The Redis cache repository instance.</param>
        public RedisCacheService(IRedisCacheRepository redisCacheRepository)
        {
            _redisCacheRepository = redisCacheRepository ?? throw new ArgumentNullException(nameof(redisCacheRepository));
        }

        /// <summary>
        /// Deletes all cache keys that match the specified search pattern.
        /// </summary>
        public async Task<bool> DeleteKeysAsync(string search, CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.DeleteKeysAsync(search, cancellationToken);
        }

        /// <summary>
        /// Fetches all cache keys that match the specified search pattern.
        /// </summary>
        public async Task<List<string>> FetchKeysAsync(string search, CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.FetchKeysAsync(search, cancellationToken);
        }

        /// <summary>
        /// Retrieves the cached value for the specified key.
        /// </summary>
        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.GetAsync(key, cancellationToken);
        }

        /// <summary>
        /// Retrieves all cache keys from the Redis database.
        /// </summary>
        public async Task<List<string>> GetAllKeysAsync(CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.GetAllKeysAsync(cancellationToken);
        }

        /// <summary>
        /// Refreshes the sliding expiration timeout for the specified cache key.
        /// </summary>
        public async Task<bool> RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.RefreshAsync(key, cancellationToken);
        }

        /// <summary>
        /// Sets a key-value pair in the Redis cache with configured expiration settings.
        /// </summary>
        public async Task<bool> SetAsync(RedisCacheModel.CacheKeyVal payload, CancellationToken cancellationToken = default)
        {
            return await _redisCacheRepository.SetAsync(payload, cancellationToken);
        }
    }
}
