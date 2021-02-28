using System;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Core.Models.Responses;

namespace DevOidc.Business.Interactions
{
    public class TokenRequestOidcInteraction : IOidcInteraction<IOidcTokenRequest, IOidcToken>
    {
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IJwtProvider _jwtProvider;

        public TokenRequestOidcInteraction(
            ISessionService sessionService,
            ITenantService tenantService,
            IScopeProvider scopeProvider,
            IClaimsProvider claimsProvider,
            IJwtProvider jwtProvider)
        {
            _sessionService = sessionService;
            _tenantService = tenantService;
            _scopeProvider = scopeProvider;
            _claimsProvider = claimsProvider;
            _jwtProvider = jwtProvider;
        }

        public async Task<IOidcToken> InteractionAsync(IOidcTokenRequest request)
        {
            string? code;
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

                code = await _sessionService.CreateSessionAsync(request.TenantId, user, client, scope.ScopeId, password.Scopes, audience, default);
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

                var user = await _tenantService.AuthenticateClientAsync(request.TenantId, clientCredentials.ClientId, clientCredentials.ClientSecret);
                if (user == null)
                {
                    throw new InvalidRequestException("Client secret is incorrect.");
                }

                code = await _sessionService.CreateSessionAsync(request.TenantId, user, client, scope.ScopeId, clientCredentials.Scopes, default, default);
            }
            else
            {
                code = (request is IOidcRefreshTokenRequest refreshTokenRequest) ? refreshTokenRequest.RefreshToken
                    : (request is IOidcCodeRequest codeRequest) ? codeRequest.Code
                    : default;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new InvalidRequestException();
            }

            var session = request is IOidcRefreshTokenRequest
                ? await _sessionService.GetLongLivedSessionAsync(request.TenantId, code)
                : await _sessionService.GetSessionAsync(request.TenantId, code);
            if (session == null)
            {
                throw new InvalidGrantException("Refresh token is expired. User should reauthenticate.");
            }

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(request.TenantId);
            if (encryptionProvider == null)
            {
                throw new InvalidRequestException("Unknown tenant.");
            }

            var accessToken = default(string);
            var idToken = default(string);

            if (_scopeProvider.AccessTokenRequested(session))
            {
                var accessTokenClaims = _claimsProvider.CreateAccessTokenClaims(session.User, session.Client, session.Audience);
                accessToken = _jwtProvider.CreateJwt(accessTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);
            }
            if (_scopeProvider.IdTokenRequested(session))
            {
                var idTokenClaims = _claimsProvider.CreateIdTokenClaims(session.User, session.Client, session.ScopeId, session.Nonce);
                idToken = _jwtProvider.CreateJwt(idTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);
            }

            var refreshCode = await _sessionService.CreateLongLivedSessionAsync(request.TenantId, session.User, session.Client, session.ScopeId, session.RequestedScopes, session.Audience, session.Nonce);

            return new OidcToken
            {
                TokenType = "Bearer",
                ExpiresIn = (int)session.Tenant.TokenLifetime.TotalSeconds,
                AccessToken = accessToken,
                RefreshToken = refreshCode,
                IdToken = idToken,
                Scope = string.Join(" ", session.RequestedScopes)
            };
        }
    }
}
