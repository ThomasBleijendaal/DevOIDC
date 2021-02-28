using System;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Models.Requests;
using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Request
{
    public class OidcTokenRequestModel
    {
        public IOidcTokenRequest GetRequest(string tenantId)
            => GrantType switch
            {
                "code" => new OidcCodeRequest
                {
                    Code = Code,
                    RedirectUri = RedirectUri,
                    TenantId = tenantId
                },
                "refresh_token" => new OidcRefreshTokenRequest
                {
                    RedirectUri = RedirectUri,
                    RefreshToken = RefreshToken,
                    TenantId = tenantId
                },
                "password" => new OidcPasswordFlowRequest
                {
                    Audience = Audience,
                    ClientId = ClientId,
                    Password = Password,
                    RedirectUri = RedirectUri,
                    Scope = Scope,
                    UserName = UserName,
                    TenantId = tenantId
                },
                "client_credentials" => new OidcClientCredentialsRequest
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    RedirectUri = RedirectUri,
                    Scope = Scope,
                    TenantId = tenantId
                },

                _ => throw new InvalidOperationException()
            };

        [JsonProperty("grant_type")]
        public string? GrantType { get; set; }

        [JsonProperty("redirect_uri")]
        public string? RedirectUri { get; set; }

        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string? ClientSecret { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonProperty("audience")]
        public string? Audience { get; set; }
    }
}
