using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;
using DevOidc.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace DevOidc.Services.Session
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IMemoryCache _memoryCache;

        public UserSessionService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Dictionary<string, object>?> GetClaimsByCodeAsync(string code)
        {
            await Task.Delay(100);

            if (_memoryCache.TryGetValue<Dictionary<string, object>>(code, out var claims))
            {
                return claims;
            }

            return default;
        }

        public async Task<string> StoreClaimsAsync(UserDto user, ClientDto client, ScopeDto scope)
        {
            var dict = new Dictionary<string, object>
            {
                { "sub", user.UserId },
                { "iss", $"http://localhost:7071/{client.TenantId}/" },
                { "aud", scope.ScopeId },
                { "email", user.UserName }
            };

            foreach (var claim in client.ExtraClaims)
            {
                dict.Add(claim.Key, claim.Value);
            }

            return await StoreClaimsAsync(dict);
        }

        public async Task<string> StoreClaimsAsync(Dictionary<string, object> claims)
        {
            await Task.Delay(100);

            var code = Base64UrlEncoder.Encode(Guid.NewGuid().ToByteArray());

            _memoryCache.Set(code, claims);

            return code;
        }
    }
}
