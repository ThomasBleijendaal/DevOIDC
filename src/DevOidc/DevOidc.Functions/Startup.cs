using System;
using Azure.Data.Tables;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Providers;
using DevOidc.Business.Scopes;
using DevOidc.Business.Session;
using DevOidc.Business.Tenant;
using DevOidc.Functions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Models;
using DevOidc.Functions.Validators;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Commands.Session;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Handlers.Client;
using DevOidc.Repositories.Handlers.Session;
using DevOidc.Repositories.Handlers.Tenant;
using DevOidc.Repositories.Handlers.User;
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
            var config = builder.GetContext().Configuration;

            builder.Services.AddMemoryCache();

            builder.Services.AddTransient<IJwtProvider, RS256JwtProvider>();
            builder.Services.AddTransient<IClaimsProvider, JwtClaimsProvider>();
            builder.Services.AddTransient<IScopeProvider, ScopeProvider>();

            var tableCredentials = new TableSharedKeyCredential(config["Table:AccountName"], config["Table:AccountKey"]);

            builder.Services.AddSingleton(tableCredentials);
            builder.Services.AddSingleton(new TableServiceClient(new Uri(config["Table:Uri"]), tableCredentials));

            builder.Services.AddTransient(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            builder.Services.AddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));

            builder.Services.AddTransient<ISessionService, TableStorageSessionService>();
            builder.Services.AddTransient<ITenantService, TenantService>();
            builder.Services.AddTransient<IUserService, UserService>();

            builder.Services.AddTransient<IClientManagementService, ClientManagementService>();
            builder.Services.AddTransient<ITenantManagementService, TenantManagementService>();
            builder.Services.AddTransient<IUserManagementService, UserManagementService>();

            builder.Services.AddTransient<ICommandHandler<CreateSessionCommand>, CreateSessionCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<CreateTenantCommand>, CreateTenantCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<ClaimTenantCommand>, ClaimTenantCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<DeleteSessionCommand>, DeleteSessionCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<DeleteTenantCommand>, DeleteTenantCommandHandler>();

            builder.Services.AddTransient<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<DeleteUserCommand>, DeleteUserCommandHandler>();

            builder.Services.AddTransient<ICommandHandler<CreateClientCommand>, CreateClientCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<UpdateClientCommand>, UpdateClientCommandHandler>();
            builder.Services.AddTransient<ICommandHandler<DeleteClientCommand>, DeleteClientCommandHandler>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IAuthenticationValidator, AzureAdJwtBearerValidator>();
            builder.Services.AddOptions<AzureAdConfig>().Bind(config.GetSection("AzureAd"));
        }
    }
}
