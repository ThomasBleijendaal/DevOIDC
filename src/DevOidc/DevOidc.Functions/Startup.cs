using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Business.Handlers;
using DevOidc.Business.Providers;
using DevOidc.Business.Scopes;
using DevOidc.Business.Session;
using DevOidc.Business.Tenant;
using DevOidc.Cms.Core.Models;
using DevOidc.Cms.Core.Repositories;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Resolvers;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RapidCMS.Core.Abstractions.Data;

namespace DevOidc.Functions
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddAuthorizationCore();
            services.AddSingleton<IAuthorizationHandler, VeryPermissiveAuthorizationHandler>();

            services.AddTransient<IJwtProvider, RS256JwtProvider>();
            services.AddTransient<IClaimsProvider, JwtClaimsProvider>();
            services.AddTransient<IScopeProvider, ScopeProvider>();

            var tableCredentials = new TableSharedKeyCredential(Configuration["Table:AccountName"], Configuration["Table:AccountKey"]);

            services.AddSingleton(tableCredentials);
            services.AddSingleton(new TableServiceClient(new Uri(Configuration["Table:Uri"]), tableCredentials));

            services.AddTransient(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddTransient(typeof(IReadRepository<>), typeof(ReadRepository<>));

            services.AddTransient<ISessionService, TableStorageSessionService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IClientManagementService, ClientManagementService>();
            services.AddTransient<ITenantManagementService, TenantManagementService>();
            services.AddTransient<IUserManagementService, UserManagementService>();

            services.AddTransient<ICommandHandler<CreateSessionCommand>, CreateSessionCommandHandler>();
            services.AddTransient<ICommandHandler<CreateTenantCommand>, CreateTenantCommandHandler>();
            services.AddTransient<ICommandHandler<ClaimTenantCommand>, ClaimTenantCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteSessionCommand>, DeleteSessionCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteTenantCommand>, DeleteTenantCommandHandler>();

            services.AddTransient<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteUserCommand>, DeleteUserCommandHandler>();

            services.AddTransient<ICommandHandler<CreateClientCommand>, CreateClientCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateClientCommand>, UpdateClientCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteClientCommand>, DeleteClientCommandHandler>();

            services.AddTransient<IOidcHandler<IOidcTokenRequest, IOidcToken>, TokenRequestOidcHandler>();
            services.AddTransient<IOidcHandler<IOidcTokenRequest, IOidcSession>, SessionOidcInteraction>();

            services.AddHttpContextAccessor();
            services.AddTransient<IAuthenticationValidator, JwtBearerValidator>();
            services.AddOptions<AzureAdConfig>().Bind(Configuration.GetSection("AzureAd"));

            services.AddSingleton<IUserResolver, UserResolver>();
            services.AddSingleton<IBaseUriResolver, BaseUriResolver>();
            services.AddScoped<TenantRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<ClientRepository>();

            services.AddLogging(logging => logging.AddConsole());

            services.AddRapidCMSFunctions(config =>
            {
                config.RegisterRepository<TenantCmsModel, TenantDto, TenantRepository>();
                config.RegisterRepository<UserCmsModel, UserDto, UserRepository>();
                config.RegisterRepository<ClientCmsModel, ClientDto, ClientRepository>();
            });
        }

        public void ConfigureWorker(IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseRequestLogger();

            builder.UseContextAccessor();

            builder.UseAuthentication();
            builder.UseAuthorization();

            builder.UseFunctionExecutionMiddleware();
        }
    }

    public class VeryPermissiveAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, IEntity resource)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
