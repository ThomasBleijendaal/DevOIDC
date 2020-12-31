using System.Threading.Tasks;
using DevOidc.Core.Extensions;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Models.Response;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
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

        [FunctionName(nameof(GetTokenByCode))]
        public async Task<IActionResult> GetTokenByCode(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/token")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToForm<OidcTokenRequestModel>();

            var code = requestModel.GrantType switch
            {
                "authorization_code" => requestModel.Code,
                "refresh_token" => requestModel.RefreshToken,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                return new BadRequestResult();
            }

            var session = await _sessionService.GetSessionAsync(tenantId, code);
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

            var claims = _claimsProvider.CreateClaims(session.User, session.Client, session.Scope);

            var accessToken = _jwtProvider.CreateJwt(claims, session.Tenant.TokenLifetime, encryptionProvider);
            var refreshCode = await _sessionService.CreateLongLivedSessionAsync(tenantId, session.User, session.Client, session.Scope);

            return new OkObjectResult(new TokenResponseModel
            {
                TokenType = "Bearer",
                ExpiresIn = 3599,
                ExtExpiresIn = 3599,
                AccessToken = accessToken,
                RefreshToken = refreshCode,
                IdToken = accessToken
            });
        }
    }
}
