using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Core.Models.Responses;

namespace DevOidc.Business.Handlers
{
    public class TokenRequestOidcHandler : IOidcHandler<IOidcTokenRequest, IOidcToken>
    {
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IJwtProvider _jwtProvider;
        private readonly IOidcHandler<IOidcTokenRequest, IOidcSession> _sessionHandler;

        public TokenRequestOidcHandler(
            ISessionService sessionService,
            ITenantService tenantService,
            IScopeProvider scopeProvider,
            IClaimsProvider claimsProvider,
            IJwtProvider jwtProvider,
            IOidcHandler<IOidcTokenRequest, IOidcSession> sessionHandler)
        {
            _sessionService = sessionService;
            _tenantService = tenantService;
            _scopeProvider = scopeProvider;
            _claimsProvider = claimsProvider;
            _jwtProvider = jwtProvider;
            _sessionHandler = sessionHandler;
        }

        public async Task<IOidcToken> HandleAsync(IOidcTokenRequest request)
        {
            var oidcSession = await _sessionHandler.HandleAsync(request);

            var session = request is IOidcRefreshTokenRequest
                ? await _sessionService.GetLongLivedSessionAsync(request.TenantId, oidcSession.Code)
                : await _sessionService.GetSessionAsync(request.TenantId, oidcSession.Code);
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

            var refreshCode = request is IOidcClientCredentialsRequest
                ? default
                : await _sessionService.CreateLongLivedSessionAsync(request.TenantId, session.User, session.Client, session.ScopeId, session.RequestedScopes, session.Audience, session.Nonce);

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
