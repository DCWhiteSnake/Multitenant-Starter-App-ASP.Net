using MultitenantStarter.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace MultitenantStarter.Auth.Infrastructure
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext()
        {
        }

        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
