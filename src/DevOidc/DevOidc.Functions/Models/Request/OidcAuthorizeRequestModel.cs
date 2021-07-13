using System.Collections.Generic;
using System.Linq;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Models.Requests;
using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcAuthorizeRequestModel : OidcRequestModel
    {
        public IOidcTokenRequest GetRequest(string tenantId)
            => new OidcPasswordFlowRequest
            {
                Audience = Audience,
                ClientId = ClientId,
                Password = Password,
                RedirectUri = RedirectUri,
                ResponseType = ResponseType,
                Scope = Scope,
                TenantId = tenantId,
                UserName = UserName,
                Nonce = Nonce
            };

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

        [JsonProperty("audience")]
        public string? Audience { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

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
    }
}
