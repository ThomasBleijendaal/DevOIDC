using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Extensions;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    // TODO: test CORS for SPA / Web
    public class TokenFunctions
    {
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IClaimsProvider _claimsProvider;

        public TokenFunctions(
            ISessionService sessionService,
            ITenantService tenantService,
            IJwtProvider jwtProvider,
            IClaimsProvider claimsProvider)
        {
            _sessionService = sessionService;
            _tenantService = tenantService;
            _jwtProvider = jwtProvider;
            _claimsProvider = claimsProvider;
        }

        [FunctionName(nameof(GetTokenByCodeAsync))]
        public async Task<IActionResult> GetTokenByCodeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/token")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToForm<OidcTokenRequestModel>();

            var isRefreshToken = requestModel.GrantType == "refresh_token";

            var code = isRefreshToken ? requestModel.RefreshToken : requestModel.Code;

            if (string.IsNullOrWhiteSpace(code))
            {
                return new BadRequestResult();
            }

            var session = isRefreshToken 
                ? await _sessionService.GetLongLivedSessionAsync(tenantId, code)
                : await _sessionService.GetSessionAsync(tenantId, code);
            if (session == null)
            {
                return new OkObjectResult(new ErrorResonseModel
                {
                    Error = "invalid_grant",
                    ErrorDescription = "Refresh token is expired. User should reauthenticate."
                });
            }

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return new OkObjectResult(new ErrorResonseModel
                {
                    Error = "invalid_request",
                    ErrorDescription = "Unknown tenant."
                });
            }

            var accessTokenClaims = _claimsProvider.CreateAccessTokenClaims(session.User, session.Client, session.Scope);
            var idTokenClaims = session.RequestedScopes?.Contains("offline_access") != true ? default : _claimsProvider.CreateIdTokenClaims(session.User, session.Client, session.Scope);

            var accessToken = _jwtProvider.CreateJwt(accessTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);
            var idToken = idTokenClaims == null ? null : _jwtProvider.CreateJwt(idTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);

            var refreshCode = await _sessionService.CreateLongLivedSessionAsync(tenantId, session.User, session.Client, session.Scope, session.RequestedScopes);

            return new OkObjectResult(new TokenResponseModel
            {
                TokenType = "Bearer",
                ExpiresIn = (int)session.Tenant.TokenLifetime.TotalSeconds,
                ExtExpiresIn = (int)session.Tenant.TokenLifetime.TotalSeconds,
                AccessToken = accessToken,
                RefreshToken = refreshCode,
                IdToken = idToken
            });
        }
    }
}
