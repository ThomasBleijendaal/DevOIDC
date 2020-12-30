using DevOidc.Functions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Validators;
using DevOidc.Services.Abstractions;
using DevOidc.Services.Jwt;
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

            builder.Services.AddSingleton<ITenantService, TenantService>();
            builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
            // builder.Services.AddSingleton<IJwtService, HS256JwtService>();
            builder.Services.AddSingleton<IJwtService, RS256JwtService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAuthenticationValidator, AzureAdJwtBearerValidator>();
        }
    }
}
