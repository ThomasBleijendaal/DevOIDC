using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Response
{
    public class ErrorResonseModel
    {
        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}
