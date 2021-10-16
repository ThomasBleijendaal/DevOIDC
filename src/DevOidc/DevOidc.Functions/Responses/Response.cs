using System.IO;
using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace DevOidc.Functions.Responses
{
    public static class Response
    {
        public static HttpResponseData CreateJsonResponse(this HttpRequestData data, object obj)
            => CreateStringContentResponse(data, JsonConvert.SerializeObject(obj), "application/json");

        public static HttpResponseData CreateHtmlResponse(this HttpRequestData data, string html)
            => CreateStringContentResponse(data, html, "text/html");

        public static HttpResponseData CreateUnauthorizedResponse(this HttpRequestData data, string message)
           => CreateStringContentResponse(data, message, "text/plain", HttpStatusCode.Unauthorized);

        public static HttpResponseData CreateNoContentResponse(this HttpRequestData data)
            => data.CreateResponse(HttpStatusCode.NoContent);

        public static HttpResponseData CreateBadRequestResponse(this HttpRequestData data)
            => data.CreateResponse(HttpStatusCode.BadRequest);

        public static HttpResponseData CreateNotFoundResponse(this HttpRequestData data)
            => data.CreateResponse(HttpStatusCode.NotFound);

        public static HttpResponseData CreateForbiddenResponse(this HttpRequestData data)
            => data.CreateResponse(HttpStatusCode.Forbidden);

        public static HttpResponseData CreateFoundResponse(this HttpRequestData data, string location)
        {
            var response = data.CreateResponse(HttpStatusCode.Found);
            response.Headers.Add("Location", location);
            return response;
        }

        private static HttpResponseData CreateStringContentResponse(this HttpRequestData data, string content, string contentType, HttpStatusCode status = HttpStatusCode.OK)
        {
            var response = data.CreateResponse(status);
            
            response.Body = new MemoryStream(Encoding.UTF8.GetBytes(content));
            response.Headers.Add("Content-Type", contentType);

            return response;
        }
    }
}
