using DevOidc.Functions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Validators;
using DevOidc.Services.Abstractions;
using DevOidc.Services.Providers;
using DevOidc.Services.Session;
using DevOidc.Services.Tenant;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DevOidc.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<ITenantService, MockTenantService>();
            builder.Services.AddSingleton<ISessionService, InMemorySessionService>();

            builder.Services.AddTransient<IJwtProvider, RS256JwtProvider>();
            builder.Services.AddTransient<IClaimsProvider, JwtClaimsProvider>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAuthenticationValidator, AzureAdJwtBearerValidator>();
        }
    }
}
