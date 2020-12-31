using System;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Core.Models;
using DevOidc.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace DevOidc.Services.Session
{
    public class InMemorySessionService : ISessionService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ITenantService _tenantService;

        public InMemorySessionService(
            IMemoryCache memoryCache,
            ITenantService tenantService)
        {
            _memoryCache = memoryCache;
            _tenantService = tenantService;
        }

        public async Task<string> CreateLongLivedSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope)
            => await StoreSession("lls", tenantId, user, client, scope);

        public async Task<string> CreateSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope)
            => await StoreSession("sls", tenantId, user, client, scope);

        private async Task<string> StoreSession(string type, string tenantId, UserDto user, ClientDto client, ScopeDto scope)
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                throw new InvalidOperationException("Tenant unknown");
            }

            var refreshCode = CreateCode();
            var cacheKey = $"{tenantId}-{type}-{refreshCode}";

            _memoryCache.Set(
                cacheKey,
                new SessionDto
                {
                    Client = client,
                    Scope = scope,
                    Tenant = tenant,
                    User = user
                });

            return refreshCode;
        }

        public async Task<SessionDto?> GetLongLivedSessionAsync(string tenantId, string code)
        {
            var storedSession = _memoryCache.Get<SessionDto>($"{tenantId}-lls-{code}");
            if (storedSession == null)
            {
                return default;
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var client = await _tenantService.GetClientAsync(tenantId, storedSession.Client.ClientId);
            var user = await _tenantService.GetUserAsync(tenantId, storedSession.Client.ClientId, storedSession.User.UserId);

            if (client == null || tenant == null || user == null)
            {
                return default;
            }

            var restoredSession = new SessionDto
            {
                Client = client,
                Scope = client.Scopes.First(x => x.ScopeId == storedSession.Scope.ScopeId),
                Tenant = tenant,
                User = user
            };

            return restoredSession;
        }

        public Task<SessionDto?> GetSessionAsync(string tenantId, string code) 
            => Task.FromResult(_memoryCache.TryGetValue<SessionDto>($"{tenantId}-sls-{code}", out var session) ? session : default);


        private static string CreateCode() => Base64UrlEncoder.Encode(Guid.NewGuid().ToByteArray());
    }
}
