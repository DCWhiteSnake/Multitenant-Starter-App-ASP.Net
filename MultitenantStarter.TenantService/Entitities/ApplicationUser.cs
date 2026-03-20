using Microsoft.AspNetCore.Identity;

namespace MultitenantStarter.Common.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required Guid TenantId { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}
