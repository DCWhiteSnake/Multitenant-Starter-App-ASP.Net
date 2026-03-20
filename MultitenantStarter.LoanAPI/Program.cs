using MultitenantStarter.Auth.Infrastructure;
using MultitenantStarter.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

// Add custom authentication scheme that validates tokens externally
builder.Services.AddAuthentication("ExternalToken")
    .AddScheme<AuthenticationSchemeOptions, ExternalTokenAuthenticationHandler>(
        "ExternalToken",
        options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireTenant", policy =>
        policy.RequireClaim("tenantId")
              .RequireClaim("apiKey"));
});

builder.Services.AddHttpContextAccessor();
var masterConnectionString = builder.Configuration.GetConnectionString("MasterConnection");
builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseSqlServer(masterConnectionString));

builder.Services.AddScoped<TenantDbContext>();
builder.Services.AddScoped<TenantRegistryService>();
builder.Services.AddScoped<MultitenantStarter.Investments.Infrastructure.AppDbContextFactory>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    tenantDbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
