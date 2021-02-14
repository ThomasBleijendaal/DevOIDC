using System;
using Microsoft.Azure.Functions.Worker.Pipeline;

namespace DevOidc.Functions.Extensions
{
    public static class FunctionExecutionContextExtensions
    {
        public static string GetBaseUri(this FunctionExecutionContext context, string? readTo = default)
        {
            var uri = new Uri(context.InvocationRequest.TriggerMetadata["req"].Http.Url);
            var requestUri = uri.GetComponents(UriComponents.Path | UriComponents.SchemeAndServer, UriFormat.Unescaped);

            if (string.IsNullOrWhiteSpace(readTo))
            {
                return requestUri;
            }

            return requestUri.Split(readTo)[0];
        }
    }
}
