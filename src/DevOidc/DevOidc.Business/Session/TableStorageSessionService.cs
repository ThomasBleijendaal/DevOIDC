using System;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
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

        public async Task<string> CreateLongLivedSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope, string? requestedScopes)
        {
            var createCommand = new CreateSessionCommand(tenantId, user, client, scope, requestedScopes);

            await _createSessionCommandHandler.HandleAsync(createCommand);

            return createCommand.SessionId ?? throw new ConflictException();
        }

        public async Task<string> CreateSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope, string? requestedScopes)
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
                    Scope = scope,
                    Tenant = tenant,
                    User = user,
                    RequestedScopes = requestedScopes
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

            if (client == null || user == null || tenant == null || scope == null)
            {
                return default;
            }

            await _deleteSessionCommandHandler.HandleAsync(new DeleteSessionCommand(tenantId, code));

            return new SessionDto
            {
                Client = client,
                RequestedScopes = session.RequestedScopes,
                Scope = scope,
                Tenant = tenant,
                User = user
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
