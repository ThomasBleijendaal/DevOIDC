using System;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Authentication
{
    // this class is temporary
    public static class FunctionsWorkerApplicationBuilderExtensions
    {
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

        public static IFunctionsWorkerApplicationBuilder UseRequestLogger(this IFunctionsWorkerApplicationBuilder builder)
        {
            return builder.Use(next =>
            {
                return async context =>
                {
                    await next(context);

                    var logger = context.InstanceServices.GetRequiredService<ILogger<Program>>();

                    try
                    {
                        var isHttp = context.InvocationRequest.TriggerMetadata.TryGetValue("req", out var request);

                        if (isHttp)
                        {
                            var url = request.Http?.Url;
                            var headers = request.Http?.Headers;
                            var body = request.Http?.Body?.Json ?? request.Http?.Body?.Bytes.Length.ToString();
                            var response = JsonConvert.SerializeObject(context.InvocationResult);

                            logger.LogInformation("HTTP request: URL: {url} ||| HEADERS: {headers} ||| BODY: {body} ||| RESPONSE: {response}", url, headers, body, response);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Request logger failed");
                    }
                };
            });
        }
    }
}
