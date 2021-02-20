using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcAuthorizeRequestModel : OidcRequestModel
    {
        [JsonProperty("audience")]
        public string? Audience { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

        [JsonProperty("response_type")]
        public string? ResponseType { get; set; } = "code";

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("prompt")]
        public string? Prompt { get; set; }

        public IReadOnlyDictionary<string, string?> LogInFormData() =>
            new Dictionary<string, string?>
            {
                { "client_id", ClientId },
                { "redirect_uri", RedirectUri },
                { "scope", Scope },
                { "audience", Audience },
                { "response_mode", ResponseMode },
                { "response_type", ResponseType },
                { "state", State },
                { "nonce", Nonce }
            };
    }
}
