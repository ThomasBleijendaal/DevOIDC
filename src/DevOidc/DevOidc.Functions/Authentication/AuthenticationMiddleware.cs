using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Options;

namespace DevOidc.Functions.Authentication
{
    public class AuthorizationMiddleware
    {
        public async Task InvokeAsync(FunctionExecutionContext context, FunctionExecutionDelegate next)
        {
            if (context.InvocationRequest is InvocationRequest invocation)
            {
                var req = invocation.InputData.FirstOrDefault(x => x.Name == "req");

                if (req?.Data?.Http != null)
                {
                    var funcMetadata = context.FunctionDefinition.Metadata?.FuncParamInfo.First(x => x.Name == "req");

                    if (funcMetadata != null && !funcMetadata.CustomAttributes.Any(x => x.AttributeType == typeof(AllowAnonymousAttribute)))
                    { 
                        if (!context.Items.ContainsKey(AuthenticationMiddleware.ContextUser))
                        {
                            context.InvocationResult = new HttpResponseData(HttpStatusCode.Unauthorized);
                            return;
                        }

                        // TODO: validate user?
                    }
                }
            }

            await next(context);
        }
    }

    public class AuthenticationMiddleware
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

        public async Task InvokeAsync(FunctionExecutionContext context, FunctionExecutionDelegate next)
        {
            if (context.InvocationRequest is InvocationRequest invocation)
            {
                var req = invocation.InputData.FirstOrDefault(x => x.Name == "req");

                if (req?.Data?.Http != null)
                {
                    try
                    {
                        var user = await _authenticationValidator.GetValidUserAsync(
                            _authenticationConfig.Authority, 
                            _authenticationConfig.ClientId, 
                            _authenticationConfig.ValidAudience, 
                            _authenticationConfig.ValidIssuer);

                        context.Items.Add(AuthenticationMiddleware.ContextUser, user);
                    }
                    catch { }
                }
            }

            await next(context);
        }
    }
}
