namespace MultitenantStarter.Auth.Infrastructure
{
    /// <summary>
    /// Model classes for Redis cache operations and responses.
    /// </summary>
    public class RedisCacheModel
    {
        /// <summary>
        /// Represents a cache key-value pair for storing data in Redis.
        /// </summary>
        public class CacheKeyVal
        {
            public required string Key { get; set; }
            public required string Value { get; set; }
        }

        /// <summary>
        /// Represents a cache response structure containing expiration and data information.
        /// </summary>
        public class CacheResponse
        {
            public required string AbsoluteExpiration { get; set; }
            public required string Data { get; set; }
            public required string SlidingExpiration { get; set; }
        }
    }
}
