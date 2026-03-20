using MultitenantStarter.Auth.Infrastructure.Helpers.Encryption;
using MultitenantStarter.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static MultitenantStarter.Auth.Infrastructure.RedisCacheModel;

namespace MultitenantStarter.Auth.Infrastructure
{
    public class AuthService(AppDbContextFactory _appDbContextFactory, TenantRegistryService _tenantRegistryService, JwtTokenGenerator _jwt, RedisCacheService _cacheService, IConfiguration configuration)
    {
        private readonly AppDbContextFactory _appDbContextFactory = _appDbContextFactory;

        public async Task<ServerResponse<string>> SignIn(string username, string password, CancellationToken cancellationToken)
        {
            using var dbContext = _appDbContextFactory.CreateDbContext();
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return new ServerResponse<string>
                {
                    status = false,
                    data = null,
                    message = "Invalid username/password"
                };
            }

            var tenant = await _tenantRegistryService.Get(user.TenantId);
            var token = _jwt.GenerateToken(user.Id, user.TenantId, user.UserName!, user.Email, tenant.ApiKey);

            var encryptionSecret = configuration.GetValue<string>("EncryptionSecret");
            var encrypted = EncryptionUtil.Encrypter(token, encryptionSecret ?? "");
            var value = new CacheKeyVal
            {
                Key = user.Id.ToString(),
                Value = encrypted,
            };

            var result = await _cacheService.GetAsync(value.Key);
            if (result != null) await _cacheService.DeleteKeysAsync(value.Key);

            await _cacheService.SetAsync(value, cancellationToken);
            return new ServerResponse<string>
            {
                status = !token.IsNullOrEmpty(),
                data = token,
                message = token.IsNullOrEmpty() ? "Invalid username/password" : "Login successful"
            };
        }
    }
}
