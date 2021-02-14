using System;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

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

        [FunctionName(nameof(TestOidcTokenAsync))]
        public async Task<HttpResponseData> TestOidcTokenAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/{tenantId}/{clientId}/{scope}")] HttpRequestData req, 
            FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId) || !req.Params.TryGetValue("clientId", out var clientId) || !req.Params.TryGetValue("scope", out var scope))
            {
                return Response.BadRequest();
            }

            try
            {
                var instance = new Uri(new Uri(context.GetBaseUri("test")), tenantId);
                var user = await _authenticationValidator.GetValidUserAsync(instance, clientId, scope);

                return Response.Json(user.Claims.ToDictionary(x => x.Type, x => x.Value));
            }
            catch (Exception ex)
            {
                return Response.Unauthorized(ex.Message);
            }
        }

        [FunctionName(nameof(GetUserInfoAsync))]
        public async Task<HttpResponseData> GetUserInfoAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/oidc/userinfo")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            try
            {
                var instance = new Uri(context.GetBaseUri("oidc"));
                var principal = await _authenticationValidator.GetClaimsAysnc(instance);

                var user = await _userService.GetUserByIdAsync(tenantId, principal.Claims.First(x => x.Type == "sub").Value);
                if (user == null)
                {
                    return Response.Forbidden();
                }

                var claims = _claimsProvider.CreateUserInfoClaims(user);

                return Response.Json(claims);
            }
            catch (Exception ex)
            {
                return Response.Unauthorized(ex.Message);
            }
        }
    }
}
