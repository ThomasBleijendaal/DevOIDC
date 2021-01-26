using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Response
{
    public class TokenResponseModel
    {
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("scope")] 
        public string? Scope { get; set; }

        [JsonProperty("expires_in")] 
        public int ExpiresIn { get; set; }

        [JsonProperty("ext_expires_in")] 
        public int ExtExpiresIn { get; set; }

        [JsonProperty("access_token")] 
        public string? AccessToken { get; set; }

        [JsonProperty("refresh_token")] 
        public string? RefreshToken { get; set; }

        [JsonProperty("id_token")] 
        public string? IdToken { get; set; }

        [JsonProperty("client_info")]
        public string? ClientInfo { get; set; }
    }
}
