using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcTokenRequestModel
    {
        [JsonProperty("grant_type")]
        public string? GrantType { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("redirect_uri")]
        public string? RedirectUri { get; set; }
    }
}
