using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public TestTokenFunctions(IAuthenticationValidator authenticationValidator)
        {
            _authenticationValidator = authenticationValidator;
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
                var user = await _authenticationValidator.GetClaimsAysnc(instance);

                // TODO: tenant configuration to add more stuff to this user info

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

        private static HttpResponseMessage ClaimsAsJson(System.Security.Claims.ClaimsPrincipal user)
        {
            var json = JsonConvert.SerializeObject(user.Claims.ToDictionary(x => x.Type, x => x.Value));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
