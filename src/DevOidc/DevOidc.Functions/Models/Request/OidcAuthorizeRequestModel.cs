using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcAuthorizeRequestModel
    {
        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("redirect_uri")]
        public string? RedirectUri { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

        [JsonIgnore]
        public IEnumerable<string> CustomScopes => Scopes.Except(new[] { "openid", "offline_access" });

        [JsonProperty("response_mode")]
        public string? ResponseMode { get; set; }

        [JsonProperty("response_type")]
        public string? ResponseType { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
