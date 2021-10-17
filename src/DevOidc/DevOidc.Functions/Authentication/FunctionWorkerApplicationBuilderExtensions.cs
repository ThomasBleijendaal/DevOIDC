using System;
using System.IO;
using System.Net;
using DevOidc.Functions.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevOidc.Functions.Authentication
{
    public static class FunctionsWorkerApplicationBuilderExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseAuthentication(this IFunctionsWorkerApplicationBuilder builder)
            => builder.UseMiddleware<AuthenticationMiddleware>();

        public static IFunctionsWorkerApplicationBuilder UseAuthorization(this IFunctionsWorkerApplicationBuilder builder)
            => builder.UseMiddleware<AuthorizationMiddleware>();

        public static IFunctionsWorkerApplicationBuilder UseRequestLogger(this IFunctionsWorkerApplicationBuilder builder)
            => builder.UseMiddleware(async (context, next) =>
            {
                await next();

                var logger = context.InstanceServices.GetRequiredService<ILogger<Program>>();

                try
                {
                    if (context.GetHttpRequestData() is HttpRequestData request)
                    {
                        var url = request.Url;
                        var headers = request.Headers;

                        logger.LogInformation("HTTP request: URL: {url} ||| HEADERS: {headers}", url, headers);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Request logger failed");
                }
            });

        public static IFunctionsWorkerApplicationBuilder UseCommonExceptions(this IFunctionsWorkerApplicationBuilder builder)
            => builder.UseMiddleware(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (InvalidDataException)
                {
                    context.InvokeResult(CreateResponse(context, HttpStatusCode.BadRequest));
                }
                catch (InvalidOperationException)
                {
                    context.InvokeResult(CreateResponse(context, HttpStatusCode.InternalServerError));
                }
            });

        public static IFunctionsWorkerApplicationBuilder UseContextAccessor(this IFunctionsWorkerApplicationBuilder builder)
            => builder.UseMiddleware<FunctionContextAccessorMiddleware>();

        private static HttpResponseData CreateResponse(FunctionContext context, HttpStatusCode statusCode)
            => context.GetHttpRequestData()?.CreateResponse(statusCode) ?? throw new Exception();
    }
}
