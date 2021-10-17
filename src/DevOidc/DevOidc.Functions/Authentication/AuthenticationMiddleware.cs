using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace DevOidc.Functions.Authentication
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        public const string ContextUser = "User";

        private readonly AzureAdConfig _authenticationConfig;
        private readonly IAuthenticationValidator _authenticationValidator;
        
        public AuthenticationMiddleware(
            IOptions<AzureAdConfig> authenticationConfig,
            IAuthenticationValidator authenticationValidator)
        {
            _authenticationConfig = authenticationConfig.Value;
            _authenticationValidator = authenticationValidator;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (context.GetHttpRequestData() is HttpRequestData request &&
                request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                try
                {
                    var user = await _authenticationValidator.GetValidUserAsync(
                        authorizationHeader,
                        _authenticationConfig.Authority,
                        _authenticationConfig.ClientId,
                        _authenticationConfig.ValidAudience,
                        _authenticationConfig.ValidIssuer);

                    context.Items.Add(ContextUser, user);
                }
                catch { }
            }

            await next(context);
        }
    }
}
