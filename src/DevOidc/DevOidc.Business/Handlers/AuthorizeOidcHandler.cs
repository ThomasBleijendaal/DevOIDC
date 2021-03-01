using System;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Core.Models.Responses;

namespace DevOidc.Business.Handlers
{
    public class AuthorizeOidcHandler : IOidcHandler<IOidcTokenRequest, IOidcAuthorization>
    {
        private readonly IOidcHandler<IOidcTokenRequest, IOidcSession> _sessionHandler;
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IJwtProvider _jwtProvider;

        public AuthorizeOidcHandler(
            IOidcHandler<IOidcTokenRequest, IOidcSession> sessionHandler,
            ISessionService sessionService,
            ITenantService tenantService,
            IClaimsProvider claimsProvider,
            IJwtProvider jwtProvider)
        {
            _sessionHandler = sessionHandler;
            _sessionService = sessionService;
            _tenantService = tenantService;
            _claimsProvider = claimsProvider;
            _jwtProvider = jwtProvider;
        }

        public async Task<IOidcAuthorization> HandleAsync(IOidcTokenRequest request)
        {
            if (request is IOidcPasswordFlowRequest password)
            {
                if (string.IsNullOrWhiteSpace(password.ClientId) ||
                    string.IsNullOrWhiteSpace(password.RedirectUri) ||
                    string.IsNullOrWhiteSpace(password.ResponseType) ||
                    await _tenantService.GetTenantAsync(request.TenantId) is not TenantDto tenant ||
                    await _tenantService.GetClientAsync(request.TenantId, password.ClientId) is not ClientDto client ||
                    !client.RedirectUris.Contains(request.RedirectUri!))
                {
                    throw new InvalidRequestException("Incorrect configuration was posted to callback.");
                }

                var session = await _sessionHandler.HandleAsync(password);

                if (password.ResponseType == "id_token")
                {
                    var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenant.TenantId);
                    if (encryptionProvider == null)
                    {
                        throw new InvalidRequestException("Incorrect encryption provider for tentant");
                    }

                    var sessionDetails = await _sessionService.GetSessionAsync(request.TenantId, session.Code);
                    if (sessionDetails == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var idTokenClaims = _claimsProvider.CreateIdTokenClaims(sessionDetails.User, client, sessionDetails.ScopeId, sessionDetails.Nonce);
                    return new OidcAuthorization("id_token", _jwtProvider.CreateJwt(idTokenClaims, tenant.TokenLifetime, encryptionProvider));
                }
                else if (password.ResponseType == "code")
                {
                    return new OidcAuthorization("code", session.Code);
                }
            }
            throw new InvalidRequestException("Response type not supported");
        }
    }
}
