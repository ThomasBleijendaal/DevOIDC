using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;

namespace DevOidc.Functions.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static T BindModelToQuery<T>(this HttpRequestData httpRequest) 
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(httpRequest.Query));

        public static T BindModelToForm<T>(this HttpRequestData httpRequest)
        {
            var formString = JsonConvert.DeserializeObject<StringRequestWrapper>(httpRequest.Body).String;
            var formAsQuery = HttpUtility.ParseQueryString(formString, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(formAsQuery.Cast<string>().ToDictionary(k => k, k => formAsQuery[k])));
        }
        
        private class StringRequestWrapper
        {
            [JsonProperty("string")]
            public string String { get; set; } = default!;
        }
    }
}
