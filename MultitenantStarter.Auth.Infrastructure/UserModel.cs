
namespace MultitenantStarter.Auth.Infrastructure
{
    public class UserModel
    {
        public string? Token { get; set; }
        public required string Username { get;  set; }
        public required string TenantName { get; set; }
    }
}