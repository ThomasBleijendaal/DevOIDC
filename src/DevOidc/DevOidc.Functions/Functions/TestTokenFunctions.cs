using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Extensions;
using DevOidc.Functions.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

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
        public async Task<HttpResponseMessage> TestOidcTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/{tenantId}/{clientId}/{scope}")] HttpRequest req,
            string tenantId,
            string clientId,
            string scope)
        {
            try
            {
                var instance = new Uri(req.HttpContext.GetServerBaseUri(), tenantId);
                var user = await _authenticationValidator.GetValidUserAsync(instance, clientId, scope);

                return ClaimsAsJson(user);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        [FunctionName(nameof(GetUserInfoAsync))]
        public async Task<HttpResponseMessage> GetUserInfoAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/oidc/userinfo")] HttpRequest req,
            string tenantId)
        {
            try
            {
                var instance = new Uri(req.HttpContext.GetServerBaseUri(), tenantId);
                var principal = await _authenticationValidator.GetClaimsAysnc(instance);

                var user = await _userService.GetUserByIdAsync(tenantId, principal.Claims.First(x => x.Type == "sub").Value);
                if (user == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Forbidden);
                }

                var claims = _claimsProvider.CreateUserInfoClaims(user);

                return ClaimsAsJson(claims);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        private static HttpResponseMessage ClaimsAsJson(System.Security.Claims.ClaimsPrincipal user)
        {
            var json = JsonConvert.SerializeObject(user.Claims.ToDictionary(x => x.Type, x => x.Value));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage ClaimsAsJson(IReadOnlyDictionary<string, object> claims)
        {
            var json = JsonConvert.SerializeObject(claims);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
