using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Session;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Exceptions;
using DevOidc.Repositories.Specifications.Session;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace DevOidc.Business.Session
{
    public class TableStorageSessionService : ISessionService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IReadRepository<SessionEntity> _sessionRepository;
        private readonly ITenantService _tenantService;
        private readonly ICommandHandler<DeleteSessionCommand> _deleteSessionCommandHandler;
        private readonly ICommandHandler<CreateSessionCommand> _createSessionCommandHandler;

        public TableStorageSessionService(
            IMemoryCache memoryCache,
            IReadRepository<SessionEntity> sessionRepository,
            ITenantService tenantService,
            ICommandHandler<DeleteSessionCommand> deleteSessionCommandHandler,
            ICommandHandler<CreateSessionCommand> createSessionCommandHandler)
        {
            _memoryCache = memoryCache;
            _sessionRepository = sessionRepository;
            _tenantService = tenantService;
            _deleteSessionCommandHandler = deleteSessionCommandHandler;
            _createSessionCommandHandler = createSessionCommandHandler;
        }

        public async Task<string> CreateLongLivedSessionAsync(string tenantId, UserDto user, ClientDto client, string scopeId, IEnumerable<string> requestedScopes, string? audience, string? nonce)
        {
            var createCommand = new CreateSessionCommand(tenantId, user, client, scopeId, requestedScopes, audience);

            await _createSessionCommandHandler.HandleAsync(createCommand);

            return createCommand.SessionId ?? throw new ConflictException();
        }

        public async Task<string> CreateSessionAsync(string tenantId, UserDto user, ClientDto client, string scopeId, IEnumerable<string> requestedScopes, string? audience, string? nonce)
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                throw new InvalidOperationException("Tenant unknown");
            }

            var refreshCode = Base64UrlEncoder.Encode(Guid.NewGuid().ToByteArray());
            var cacheKey = $"{tenantId}-sls-{refreshCode}";

            _memoryCache.Set(
                cacheKey,
                new SessionDto
                {
                    Client = client,
                    ScopeId = scopeId,
                    Tenant = tenant,
                    User = user,
                    RequestedScopes = requestedScopes.ToList(),
                    Nonce = nonce,
                    Audience = audience
                });

            return refreshCode;
        }

        public async Task<SessionDto?> GetLongLivedSessionAsync(string tenantId, string code)
        {
            var session = await _sessionRepository.GetAsync(new GetSessionSpecification(tenantId, code));
            if (session == null)
            {
                return default;
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            var user = await _tenantService.GetUserAsync(tenantId, session.ClientId, session.UserId);
            var client = await _tenantService.GetClientAsync(tenantId, session.ClientId);
            var scope = client?.Scopes.FirstOrDefault(x => x.ScopeId == session.ScopeId);

            // check if the session is useful to restore
            if (client == null || user == null || tenant == null || scope == null)
            {
                return default;
            }

            await _deleteSessionCommandHandler.HandleAsync(new DeleteSessionCommand(tenantId, code));

            return new SessionDto
            {
                Client = client,
                RequestedScopes = session.RequestedScopes.ToList(),
                ScopeId = scope.ScopeId,
                Tenant = tenant,
                User = user,
                Nonce = session.Nonce,
                Audience = session.Audience
            };
        }

        public Task<SessionDto?> GetSessionAsync(string tenantId, string code)
        {
            var cacheKey = $"{tenantId}-sls-{code}";
            if (_memoryCache.TryGetValue<SessionDto>(cacheKey, out var session))
            {
                _memoryCache.Remove(cacheKey);

                return Task.FromResult<SessionDto?>(session);
            }

            return Task.FromResult<SessionDto?>(default);
        }
    }
}
