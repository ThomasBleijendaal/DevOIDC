using System.Collections.Generic;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Extensions;
using DevOidc.Core.Models;
using Microsoft.AspNetCore.Http;

namespace DevOidc.Business.Providers
{
    public class JwtClaimsProvider : IClaimsProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, object> CreateAccessTokenClaims(UserDto user, ClientDto client, string? audience)
        {
            var baseUri = _httpContextAccessor.HttpContext.GetServerBaseUri();

            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "iss", $"{baseUri}{client.TenantId}" },
                { "email", user.UserName },
                { "name", user.FullName },
                { "__dev-oidc-token_type", "access-token" }
            };

            if (!string.IsNullOrWhiteSpace(audience))
            {
                dict.Add("aud", audience!);
            }

            AddClaims(client.AccessTokenExtraClaims, dict);
            AddClaims(user.AccessTokenExtraClaims, dict);

            return dict;
        }

        public Dictionary<string, object> CreateIdTokenClaims(UserDto user, ClientDto client, string scope, string? nonce)
        {
            var baseUri = _httpContextAccessor.HttpContext.GetServerBaseUri();

            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "iss", $"{baseUri}{client.TenantId}" },
                { "aud", scope },
                { "name", user.FullName },
                { "__dev-oidc-token_type", "id-token" }
            };

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                dict.Add("nonce", nonce!);
            }

            AddClaims(client.IdTokenExtraClaims, dict);
            AddClaims(user.IdTokenExtraClaims, dict);

            return dict;
        }

        public Dictionary<string, object> CreateUserInfoClaims(UserDto user)
        {
            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "name", user.FullName },
                { "__dev-oidc-token_type", "user-info-token" }
            };

            AddClaims(user.IdTokenExtraClaims, dict);
            AddClaims(user.UserInfoExtraClaims, dict);

            return dict;
        }

        private static void AddClaims(IReadOnlyDictionary<string, string> claims, Dictionary<string, object> dict)
        {
            foreach (var claim in claims)
            {
                dict.Add(claim.Key, claim.Value);
            }
        }
    }
}
