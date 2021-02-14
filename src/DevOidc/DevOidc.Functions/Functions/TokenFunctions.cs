using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Models.Response;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    // TODO: test nonce for refesh tokens
    public class TokenFunctions
    {
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IScopeProvider _scopeProvider;

        public TokenFunctions(
            ISessionService sessionService,
            ITenantService tenantService,
            IJwtProvider jwtProvider,
            IClaimsProvider claimsProvider,
            IScopeProvider scopeProvider)
        {
            _sessionService = sessionService;
            _tenantService = tenantService;
            _jwtProvider = jwtProvider;
            _claimsProvider = claimsProvider;
            _scopeProvider = scopeProvider;
        }

        [FunctionName(nameof(GetTokenByCodeAsync))]
        public async Task<HttpResponseData> GetTokenByCodeAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/token")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var requestModel = req.BindModelToForm<OidcTokenRequestModel>();

            var isRefreshToken = requestModel.GrantType == "refresh_token";

            var code = isRefreshToken ? requestModel.RefreshToken : requestModel.Code;

            if (string.IsNullOrWhiteSpace(code))
            {
                return Response.BadRequest();
            }

            var session = isRefreshToken
                ? await _sessionService.GetLongLivedSessionAsync(tenantId, code)
                : await _sessionService.GetSessionAsync(tenantId, code);
            if (session == null)
            {
                return Response.Json(new ErrorResonseModel
                {
                    Error = "invalid_grant",
                    ErrorDescription = "Refresh token is expired. User should reauthenticate."
                });
            }

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return Response.Json(new ErrorResonseModel
                {
                    Error = "invalid_request",
                    ErrorDescription = "Unknown tenant."
                });
            }

            var accessToken = default(string);
            var idToken = default(string);

            if (_scopeProvider.AccessTokenRequested(session))
            {
                var accessTokenClaims =_claimsProvider.CreateAccessTokenClaims(session.User, session.Client, session.Audience);
                accessToken = _jwtProvider.CreateJwt(accessTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);
            }
            if (_scopeProvider.IdTokenRequested(session))
            {
                var idTokenClaims = _claimsProvider.CreateIdTokenClaims(session.User, session.Client, session.ScopeId, session.Nonce);
                idToken = _jwtProvider.CreateJwt(idTokenClaims, session.Tenant.TokenLifetime, encryptionProvider);
            }

            var refreshCode = await _sessionService.CreateLongLivedSessionAsync(tenantId, session.User, session.Client, session.ScopeId, session.RequestedScopes, session.Audience, session.Nonce);

            return Response.Json(new TokenResponseModel
            {
                TokenType = "Bearer",
                ExpiresIn = (int)session.Tenant.TokenLifetime.TotalSeconds,
                ExtExpiresIn = (int)session.Tenant.TokenLifetime.TotalSeconds,
                AccessToken = accessToken,
                RefreshToken = refreshCode,
                IdToken = idToken,
                Scope = string.Join(" ", session.RequestedScopes)
            });
        }
    }
}
