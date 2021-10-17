using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace DevOidc.Functions.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static T BindModelToQuery<T>(this HttpRequestData httpRequest)
        {
            var qs = HttpUtility.ParseQueryString(httpRequest.Url.Query);

            var json = JsonConvert.SerializeObject(qs.Cast<string>().ToDictionary(k => k, k => qs[k]));

            return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidDataException();
        }

        public static T BindModelToForm<T>(this HttpRequestData httpRequest)
        {
            using var streamReader = new StreamReader(httpRequest.Body);

            var formString = streamReader.ReadToEnd();
            var formAsQuery = HttpUtility.ParseQueryString(formString, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(formAsQuery.Cast<string>().ToDictionary(k => k, k => formAsQuery[k]))) ?? throw new InvalidDataException();
        }
        
        public static bool TryGetValue(this HttpHeaders headers, string header, out string headerValue)
        {
            if (headers.TryGetValues(header, out var values) && values.Any())
            {
                headerValue = values.First();
                return true;
            }

            headerValue = "";
            return false;
        }
    }
}
