using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DevOidc.Functions.Extensions
{
    public static class FunctionExecutionContextExtensions
    {
        public static string GetBaseUri(this FunctionContext context, string? readTo = default)
        {
            var uri = context.GetHttpRequestData()?.Url;
            if (uri == null)
            {
                return "";
            }

            var requestUri = uri.GetComponents(UriComponents.Path | UriComponents.SchemeAndServer, UriFormat.Unescaped);

            if (string.IsNullOrWhiteSpace(readTo))
            {
                return requestUri;
            }

            return requestUri.Split(readTo)[0];
        }

        public static IEnumerable<TAttribute> GetFunctionAttributes<TAttribute>(this FunctionContext context)
            where TAttribute : Attribute
        {
            // TODO: should the function class itself be checked for attributes?

            var entryPointData = context.FunctionDefinition.EntryPoint.Split('.');
            if (entryPointData.Length <= 1)
            {
                return Enumerable.Empty<TAttribute>();
            }

            var typeName = string.Join(".", entryPointData[0..^1]);
            var methodName = entryPointData[^1];

            var type = Assembly.GetEntryAssembly()?.GetType(typeName);
            if (type == null)
            {
                return Enumerable.Empty<TAttribute>();
            }

            var method = type.GetMethod(methodName);
            if (method == null)
            {
                return Enumerable.Empty<TAttribute>();
            }

            return method.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
        }

        public static HttpRequestData? GetHttpRequestData(this FunctionContext context)
        {
            var functionBindingsFeature = GetBindingsFeature(context);
            if (functionBindingsFeature == null)
            {
                return null;
            }

            var type = functionBindingsFeature.GetType();
            var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
            return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
        }

        public static void InvokeResult(this FunctionContext context, HttpResponseData response)
        {
            var functionBindingsFeature = GetBindingsFeature(context);
            if (functionBindingsFeature == null)
            {
                return;
            }

            var type = functionBindingsFeature.GetType();
            var result = type.GetProperties().Single(p => p.Name == "InvocationResult");
            result.SetValue(functionBindingsFeature, response);
        }

        private static object? GetBindingsFeature(FunctionContext context)
        {
            var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            var functionBindingsFeature = keyValuePair.Value;
            return functionBindingsFeature;
        }
    }
}
