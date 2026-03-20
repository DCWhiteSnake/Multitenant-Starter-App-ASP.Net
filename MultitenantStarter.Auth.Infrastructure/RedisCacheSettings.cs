namespace MultitenantStarter.Auth.Infrastructure
{
    /// <summary>
    /// Configuration settings for Redis cache connection and expiration policies.
    /// </summary>
    public class RedisCacheSettings
    {
        /// <summary>
        /// Gets or sets the Redis server host address.
        /// </summary>
        public required string Host { get; set; }
        
        /// <summary>
        /// Gets or sets the Redis server port number.
        /// </summary>
        public required string Port { get; set; }
        
        /// <summary>
        /// Gets or sets the Redis server authentication password.
        /// </summary>        
        public required string Password { get; set; }
        
        /// <summary>
        /// Gets or sets the absolute expiration time in hours relative to now for cached items.
        /// </summary>       
        public required int AbsoluteExpirationInSeconds { get; set; }
        
        /// <summary>
        /// Gets or sets the sliding expiration time in minutes for cached items.
        /// The expiration is reset each time the item is accessed.
        /// </summary>        
        public required int SlidingExpirationInSeconds { get; set; }


    }
}
