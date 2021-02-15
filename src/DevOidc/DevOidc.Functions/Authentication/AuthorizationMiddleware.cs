using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;

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
}
