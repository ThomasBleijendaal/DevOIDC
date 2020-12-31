using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DevOidc.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static Uri GetServerBaseUri(this HttpContext httpContext)
        {
            return new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
        }

        public static Uri GetRequestUri(this HttpContext httpContext)
        {
            return new Uri(GetServerBaseUri(httpContext), httpContext.Request.Path.ToString());
        }
    }

    public static class HttpRequestExtensions
    {
        public static T BindModelToQuery<T>(this HttpRequest httpRequest)
        {
            var queryDict = httpRequest.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(queryDict));
        }

        public static T BindModelToForm<T>(this HttpRequest httpRequest)
        {
            var formDict = httpRequest.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(formDict));
        }
    }
}
