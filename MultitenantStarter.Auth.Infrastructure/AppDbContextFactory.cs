using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MultitenantStarter.Auth.Infrastructure
{
    public class AppDbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantRegistryService _tenantRegistryService;

        public AppDbContextFactory(IHttpContextAccessor httpContextAccessor, TenantRegistryService tenantRegistryService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantRegistryService = tenantRegistryService;
        }

        public AppDbContext CreateDbContext()
        {
            // ASP.NET Core's Headers dictionary is case-insensitive
            string? tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["tenantid"].ToString();
            string? apiKeyHeader = _httpContextAccessor.HttpContext?.Request.Headers["apikey"].ToString();

            if (string.IsNullOrEmpty(tenantIdHeader) || !Guid.TryParse(tenantIdHeader, out var tenantId))
            {
                throw new InvalidOperationException("Tenant ID is not available in the current context.");
            }

            if (string.IsNullOrEmpty(apiKeyHeader))
            {
                throw new InvalidOperationException("API Key is not available in the current context.");
            }

            var connString = _tenantRegistryService.GetConnectionString(tenantId, apiKeyHeader);
            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException("Connection string could not be retrieved for the tenant.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connString);

            var dbContext = Activator.CreateInstance(typeof(AppDbContext), optionsBuilder.Options) as AppDbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("Failed to create an instance of AppDbContext.");
            }

            return dbContext;
        }
    }
}
