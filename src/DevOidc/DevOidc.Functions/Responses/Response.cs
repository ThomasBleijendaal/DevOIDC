using System.Net;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;

namespace DevOidc.Functions.Responses
{
    public static class Response
    {
        public static HttpResponseData Json(object obj)
            => StringContent(JsonConvert.SerializeObject(obj), "application/json");

        public static HttpResponseData Html(string html)
            => StringContent(html, "text/html");

        public static HttpResponseData Unauthorized(string message)
           => StringContent(message, "text/plain", HttpStatusCode.Unauthorized);

        public static HttpResponseData NoContent()
            => new HttpResponseData(HttpStatusCode.NoContent);

        public static HttpResponseData BadRequest()
            => new HttpResponseData(HttpStatusCode.BadRequest);

        public static HttpResponseData NotFound()
            => new HttpResponseData(HttpStatusCode.NotFound);

        public static HttpResponseData Forbidden()
            => new HttpResponseData(HttpStatusCode.Forbidden);

        public static HttpResponseData Found(string location)
        {
            var response = new HttpResponseData(HttpStatusCode.Found);
            response.Headers.Add("Location", location);
            return response;
        }

        private static HttpResponseData StringContent(string content, string contentType, HttpStatusCode status = HttpStatusCode.OK)
        {
            var response = new HttpResponseData(status, content);
            response.Headers.Add("Content-Type", contentType);
            return response;
        }
    }
}
