using System.Collections.Generic;
using DevOidc.Core.Extensions;
using DevOidc.Core.Models;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace DevOidc.Services.Providers
{
    public class JwtClaimsProvider : IClaimsProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, object> CreateAccessTokenClaims(UserDto user, ClientDto client, ScopeDto scope)
        {
            var baseUri = _httpContextAccessor.HttpContext.GetServerBaseUri();

            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "iss", $"{baseUri}{client.TenantId}/" },
                { "aud", scope.ScopeId },
                { "email", user.UserName }
            };

            AddClaims(client.ExtraClaims, dict);
            AddClaims(user.ExtraClaims, dict);

            return dict;
        }

        public Dictionary<string, object> CreateIdTokenClaims(UserDto user, ClientDto client, ScopeDto scope)
        {
            var baseUri = _httpContextAccessor.HttpContext.GetServerBaseUri();

            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "iss", $"{baseUri}{client.TenantId}/" },
                { "aud", scope.ScopeId },
                { "name", user.FullName }
            };

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
