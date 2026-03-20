using MultitenantStarter.Auth.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>();

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<JwtTokenGenerator>();

var masterConnectionString = builder.Configuration.GetConnectionString("MasterConnection");
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseSqlServer(masterConnectionString));

builder.Services.AddOptions<RedisCacheSettings>()
    .Bind(builder.Configuration.GetSection("RedisCacheSettings"))
    .ValidateDataAnnotations();

var redisSettings = builder.Configuration.GetSection("RedisCacheSettings").Get<RedisCacheSettings>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{redisSettings?.Host}:{redisSettings?.Port},password={redisSettings?.Password}";
});

builder.Services.AddSingleton<IRedisCacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddScoped<TenantRegistryService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AppDbContextFactory>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
