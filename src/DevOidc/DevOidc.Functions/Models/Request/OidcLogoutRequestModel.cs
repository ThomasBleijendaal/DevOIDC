using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcLogoutRequestModel
    {
        [JsonProperty("post_logout_redirect_uri")]
        public string? LogoutRedirectUri { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }
    }
}
