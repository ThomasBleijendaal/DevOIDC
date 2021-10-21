using System;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DevOidc.Functions.Functions
{
    public class TestTokenFunctions
    {
        private readonly IAuthenticationValidator _authenticationValidator;
        private readonly IUserService _userService;
        private readonly IClaimsProvider _claimsProvider;

        public TestTokenFunctions(
            IAuthenticationValidator authenticationValidator,
            IUserService userService,
            IClaimsProvider claimsProvider)
        {
            _authenticationValidator = authenticationValidator;
            _userService = userService;
            _claimsProvider = claimsProvider;
        }

        [Function(nameof(TestOidcTokenAsync))]
        [AllowAnonymous]
        public async Task<HttpResponseData> TestOidcTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/{tenantId}/{clientId}/{scope}")] HttpRequestData req, 
            string tenantId,
            string clientId,
            string scope,
            FunctionContext context)
        {
            try
            {
                var authorizationValue = req.Headers.TryGetValue("Authorization", out var headerValue) ? headerValue : "";
                var instance = new Uri(new Uri(context.GetBaseUri("/test")), tenantId);
                var user = await _authenticationValidator.GetValidUserAsync(authorizationValue, instance, clientId, scope);

                return req.CreateJsonResponse(user.Claims.ToDictionary(x => x.Type, x => x.Value));
            }
            catch (Exception ex)
            {
                return req.CreateUnauthorizedResponse(ex.Message);
            }
        }

        [Function(nameof(GetUserInfoAsync))]
        [AllowAnonymous]
        public async Task<HttpResponseData> GetUserInfoAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/oidc/userinfo")] HttpRequestData req, 
            string tenantId,
            FunctionContext context)
        {
            try
            {
                var authorizationValue = req.Headers.TryGetValue("Authorization", out var headerValue) ? headerValue : "";
                var instance = new Uri(context.GetBaseUri("/oidc"));
                var principal = await _authenticationValidator.GetClaimsAysnc(authorizationValue, instance);

                var user = await _userService.GetUserByIdAsync(tenantId, principal.Claims.First(x => x.Type == "sub").Value);
                if (user == null)
                {
                    return req.CreateForbiddenResponse();
                }

                var claims = _claimsProvider.CreateUserInfoClaims(user);

                return req.CreateJsonResponse(claims);
            }
            catch (Exception ex)
            {
                return req.CreateUnauthorizedResponse(ex.Message);
            }
        }
    }
}
