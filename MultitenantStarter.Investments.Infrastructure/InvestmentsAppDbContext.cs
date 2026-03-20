using MultitenantStarter.Common.Entities;
using MultitenantStarter.Common.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MultitenantStarter.Investments.Infrastructure
{
    public class InvestmentsAppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public InvestmentsAppDbContext(DbContextOptions<InvestmentsAppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
