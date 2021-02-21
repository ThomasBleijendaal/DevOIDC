using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcTokenRequestModel
    {
        [JsonProperty("grant_type")]
        public string? GrantType { get; set; }

        [JsonProperty("redirect_uri")]
        public string? RedirectUri { get; set; }

        // code flow
        [JsonProperty("code")]
        public string? Code { get; set; }

        // refresh token flow
        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        // password flow
        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

        [JsonProperty("audience")]
        public string? Audience { get; set; }
    }
}
