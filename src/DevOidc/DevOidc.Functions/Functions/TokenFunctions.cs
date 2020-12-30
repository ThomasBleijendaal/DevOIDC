using System.Threading.Tasks;
using DevOidc.Functions.Models;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class TokenFunctions
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IJwtService _jwtService;

        public TokenFunctions(
            IUserSessionService userSessionService,
            IJwtService jwtService)
        {
            _userSessionService = userSessionService;
            _jwtService = jwtService;
        }

        [FunctionName(nameof(GetTokenByCode))]
        public async Task<IActionResult> GetTokenByCode(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/token")] HttpRequest req,
            string tenantId)
        {
            var form = await req.ReadFormAsync();
            var grantType = form["grant_type"].ToString();

            var code = grantType switch
            {
                "authorization_code" => form["code"].ToString(),
                "refresh_token" => form["refresh_token"].ToString(),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                return new BadRequestResult();
            }

            var claims = await _userSessionService.GetClaimsByCodeAsync(tenantId, code);
            if (claims == null)
            {
                return new NotFoundResult();
            }

            var refreshToken = await _userSessionService.StoreClaimsAsync(tenantId, claims);
            var accessToken = _jwtService.CreateJwt(claims);

            return new OkObjectResult(new TokenResponseModel
            {
                TokenType = "Bearer",
                ExpiresIn = 3599,
                ExtExpiresIn = 3599,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IdToken = accessToken
            });
        }
    }
}
