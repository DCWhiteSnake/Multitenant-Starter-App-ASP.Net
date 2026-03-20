using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using MultitenantStarter.Auth.Infrastructure;
using MultitenantStarter.Auth.Infrastructure.Helpers.Encryption;
using MultitenantStarter.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using TenantRegistryService = MultitenantStarter.Auth.Infrastructure.TenantRegistryService;

namespace MultitenantStarter.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ILogger<AuthController> _logger, AuthService _authService, TenantRegistryService _tenantRegistryService, RedisCacheService _cacheService, IConfiguration configuration) : ControllerBase
    {
        [HttpPost("log-in")]
        public async Task<IActionResult> SignIn([FromHeader] string apiKey, [FromHeader] string tenantId, [FromForm] string username, [FromForm] string password, RedisCacheService cacheService, CancellationToken cancellationToken)
        {
            var response = await _authService.SignIn(username, password, cancellationToken);
            if (response.status) return Ok(response);
            return Unauthorized();
        }

        [HttpPost("register-tenant")]
        public IActionResult RegisterTenant(string organizationName)
        {
            var tenant = new Tenant
            {
                Name = organizationName,
                ConnectionString = "Server=.;Database=TenantDb_" + organizationName + Guid.NewGuid().ToString() + ";Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;",
                ApiKey = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid()
            };
            _tenantRegistryService.RegisterTenant(tenant);
            return Ok(new { message = "Please keep these details as they will not be provided again", apiKey = tenant.ApiKey, username = organizationName, password = "Secret123$", TenantId = tenant.Id });
        }

        [HttpPost("validate-jwt")]
        public async Task<IActionResult> ValidateJWT([FromForm] string jwt, CancellationToken cancellationToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(jwt))
                {
                    return Unauthorized();
                }

                var jwtToken = handler.ReadJwtToken(jwt);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                var cachedData = await _cacheService.GetAsync(userId ?? "", cancellationToken);
                if (cachedData == null) return Unauthorized();

                cachedData = Regex.Unescape(cachedData);
                // Strip surrounding JSON quotes added by JsonSerializer.Serialize on a plain string
                if (cachedData.Length >= 2 && cachedData.StartsWith('"') && cachedData.EndsWith('"'))
                {
                    cachedData = cachedData.Substring(1, cachedData.Length - 2);
                }

                var encryptionSecret = configuration.GetValue<string>("EncryptionSecret");
                var decrypted = EncryptionUtil.DecryptText(cachedData, encryptionSecret ?? "");

                if (decrypted != jwt)
                {
                    return Unauthorized();
                }

                var tenantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                var apiKey = jwtToken.Claims.FirstOrDefault(c => c.Type == "apiKey")?.Value;
                var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

                return Ok(new AuthTokenModel(
                    true,
                    tenantId ?? "",
                    apiKey ?? "",
                    userId ?? "",
                    username ?? ""));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT");
                return Unauthorized();
            }
        }
    }

    public record AuthTokenModel(bool IsValid,
                                 string TenantId,
                                 string ApiKey,
                                 string UserId,
                                 string Username);
}
