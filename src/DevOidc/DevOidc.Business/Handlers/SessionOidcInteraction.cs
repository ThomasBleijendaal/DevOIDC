using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Core.Models.Responses;

namespace DevOidc.Business.Handlers
{
    public class SessionOidcInteraction : IOidcHandler<IOidcTokenRequest, IOidcSession>
    {
        private readonly ITenantService _tenantService;
        private readonly ISessionService _sessionService;

        public SessionOidcInteraction(
            ITenantService tenantService,
            ISessionService sessionService)
        {
            _tenantService = tenantService;
            _sessionService = sessionService;
        }

        public async Task<IOidcSession> HandleAsync(IOidcTokenRequest request)
        {
            if (request is IOidcPasswordFlowRequest password)
            {
                if (string.IsNullOrEmpty(password.ClientId) ||
                    string.IsNullOrEmpty(password.UserName) ||
                    string.IsNullOrEmpty(password.Password) ||
                    await _tenantService.GetClientAsync(password.TenantId, password.ClientId) is not ClientDto client)
                {
                    throw new InvalidRequestException();
                }

                var scope = client.Scopes.FirstOrDefault(x => password.Scopes.Contains(x.ScopeId))
                    ?? new ScopeDto { ScopeId = password.ClientId };

                var audience = !string.IsNullOrWhiteSpace(password.Audience) ? password.Audience : scope.ScopeId;

                var user = await _tenantService.AuthenticateUserAsync(request.TenantId, password.ClientId, password.UserName, password.Password);
                if (user == null)
                {
                    throw new InvalidRequestException("Username or password is incorrect, or user does not have access to this client.");
                }

                return new OidcSession(await _sessionService.CreateSessionAsync(request.TenantId, user, client, scope.ScopeId, password.Scopes, audience, default));
            }
            else if (request is IOidcClientCredentialsRequest clientCredentials)
            {
                if (string.IsNullOrEmpty(clientCredentials.ClientId) ||
                    string.IsNullOrEmpty(clientCredentials.ClientSecret) ||
                    await _tenantService.GetClientAsync(clientCredentials.TenantId, clientCredentials.ClientId) is not ClientDto client)
                {
                    throw new InvalidRequestException();
                }

                var scope = client.Scopes.FirstOrDefault(x => clientCredentials.Scopes.Contains(x.ScopeId))
                    ?? new ScopeDto { ScopeId = clientCredentials.ClientId };

                var clientUser = await _tenantService.AuthenticateClientAsync(request.TenantId, clientCredentials.ClientId, clientCredentials.ClientSecret);
                if (clientUser == null)
                {
                    throw new InvalidRequestException("Client secret is incorrect.");
                }

                var user = new UserDto
                {
                    AccessTokenExtraClaims = new Dictionary<string, string>(),
                    Clients = new List<string>
                    {
                        clientUser.ClientId
                    },
                    FullName = clientUser.ClientId,
                    IdTokenExtraClaims = new Dictionary<string, string>(),
                    Password = string.Empty,
                    UserId = clientUser.ClientId,
                    UserInfoExtraClaims = new Dictionary<string, string>(),
                    UserName = clientUser.ClientId
                };

                return new OidcSession(await _sessionService.CreateSessionAsync(request.TenantId, user, client, scope.ScopeId, clientCredentials.Scopes, default, default));
            }
            else if (request is IOidcRefreshTokenRequest refreshTokenRequest)
            {
                return new OidcSession(refreshTokenRequest.RefreshToken ?? throw new InvalidRequestException());
                ;
            }
            else if (request is IOidcCodeRequest codeRequest)
            {
                return new OidcSession(codeRequest.Code ?? throw new InvalidRequestException());
            }

            throw new InvalidRequestException();
        }
    }
}
