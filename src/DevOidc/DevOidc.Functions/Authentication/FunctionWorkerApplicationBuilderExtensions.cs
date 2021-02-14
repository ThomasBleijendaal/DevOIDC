using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Authentication
{
    public static class FunctionsWorkerApplicationBuilderExtensions
    {
        // this class is temporary
        public static IFunctionsWorkerApplicationBuilder UseAuthentication(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AuthenticationMiddleware>();

            return builder.Use(next =>
            {
                return context =>
                {
                    var middleware = context.InstanceServices.GetRequiredService<AuthenticationMiddleware>();
                    return middleware.InvokeAsync(context, next);
                };
            });
        }

        public static IFunctionsWorkerApplicationBuilder UseAuthorization(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AuthorizationMiddleware>();

            return builder.Use(next =>
            {
                return context =>
                {
                    var middleware = context.InstanceServices.GetRequiredService<AuthorizationMiddleware>();
                    return middleware.InvokeAsync(context, next);
                };
            });
        }

        public static IFunctionsWorkerApplicationBuilder UseContextAccessor(this IFunctionsWorkerApplicationBuilder builder)
        {
            return builder.Use(next =>
            {
                return context =>
                {
                    var accessor = context.InstanceServices.GetRequiredService<IFunctionExecutionContextAccessor>();
                    accessor.FunctionExecutionContext = context;

                    return next(context);
                };
            });
        }
    }
}
