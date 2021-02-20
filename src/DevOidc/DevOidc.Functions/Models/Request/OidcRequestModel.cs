using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public abstract class OidcRequestModel
    {
        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("redirect_uri")]
        public string? RedirectUri { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonProperty("response_mode")]
        public string? ResponseMode { get; set; } = "query";

        [JsonProperty("response_type")]
        public string? ResponseType { get; set; } = "code";

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("session_state")]
        public string? SessionState { get; set; }
    }
}
