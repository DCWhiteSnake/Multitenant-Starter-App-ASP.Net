using System;
using MultitenantStarter.Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MultitenantStarter.Common
{
    public class SeedApplicationUser : IdentityUser<Guid>
    {
        public Guid TenantId { get; set; }
    }

    public class TenantAppIdentityDbContext : IdentityDbContext<SeedApplicationUser, IdentityRole<Guid>, Guid>
    {
        public TenantAppIdentityDbContext(DbContextOptions<TenantAppIdentityDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
