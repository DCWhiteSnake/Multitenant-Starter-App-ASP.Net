namespace MultitenantStarter.Auth.Infrastructure
{
    /// <summary>
    /// Interface for Redis cache repository operations.
    /// </summary>
    public interface IRedisCacheRepository
    {
        Task<bool> DeleteKeysAsync(string search, CancellationToken cancellationToken = default);
        Task<List<string>> FetchKeysAsync(string search, CancellationToken cancellationToken = default);
        Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<List<string>> GetAllKeysAsync(CancellationToken cancellationToken = default);
        Task<bool> RefreshAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> SetAsync(RedisCacheModel.CacheKeyVal payload, CancellationToken cancellationToken = default);
    }
}
