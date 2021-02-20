using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcAuthorizeCallbackRequestModel : OidcRequestModel
    {
        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("id_token")]
        public string? IdToken { get; set; }
    }
}
