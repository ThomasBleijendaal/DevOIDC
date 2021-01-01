using Azure.Data.Tables;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Providers;
using DevOidc.Business.Session;
using DevOidc.Business.Tenant;
using DevOidc.Functions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Validators;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using DevOidc.Repositories.Handlers;
using DevOidc.Repositories.Repositories;
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

            builder.Services.AddTransient<IJwtProvider, RS256JwtProvider>();
            builder.Services.AddTransient<IClaimsProvider, JwtClaimsProvider>();

            builder.Services.AddSingleton(new TableServiceClient("UseDevelopmentStorage=true"));

            builder.Services.AddTransient(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            builder.Services.AddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));

            builder.Services.AddTransient<ISessionService, TableStorageSessionService>();
            builder.Services.AddTransient<ITenantService, TenantService>();
            builder.Services.AddTransient<ITenantManagementService, TenantManagementService>();

            builder.Services.AddTransient<ICommandHandler<CreateSessionCommand>, CreateSessionCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<DeleteSessionCommand>, DeleteSessionCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<CreateTenantCommand>, CreateTenantCommandHandler>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAuthenticationValidator, AzureAdJwtBearerValidator>();
        }
    }
}
