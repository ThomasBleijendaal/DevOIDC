using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DevOidc.Functions.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Authentication
{
    public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (context.GetHttpRequestData() is HttpRequestData httpRequestData)
            {
                var attributes = context.GetFunctionAttributes<AllowAnonymousAttribute>();

                if (!attributes.Any() && !context.Items.Any(x => x.Key.Equals(AuthenticationMiddleware.ContextUser)))
                {
                    context.InvokeResult(httpRequestData.CreateResponse(HttpStatusCode.Unauthorized));
                    return;
                }
            }

            await next(context);
        }
    }

    public class FunctionContextAccessorMiddleware : IFunctionsWorkerMiddleware
    {
        private IFunctionContextAccessor FunctionContextAccessor { get; }

        public FunctionContextAccessorMiddleware(IFunctionContextAccessor accessor)
        {
            FunctionContextAccessor = accessor;
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (FunctionContextAccessor.FunctionExecutionContext != null)
            {
                // This should never happen because the context should be localized to the current Task chain.
                // But if it does happen (perhaps the implementation is bugged), then we need to know immediately so it can be fixed.
                throw new InvalidOperationException($"Unable to initalize {nameof(IFunctionContextAccessor)}: context has already been initialized.");
            }

            FunctionContextAccessor.FunctionExecutionContext = context;

            return next(context);
        }
    }
}
