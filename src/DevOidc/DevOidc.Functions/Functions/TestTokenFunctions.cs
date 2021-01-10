using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevOidc.Core.Extensions;
using DevOidc.Functions.Abstractions;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> TestOidcTokenAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/{tenantId}/{clientId}/{scope}")] HttpRequest req,
            string tenantId,
            string clientId,
            string scope)
        {
            try
            {
                var instance = new Uri(req.HttpContext.GetServerBaseUri(), tenantId);
                var user = await _authenticationValidator.GetValidUserAsync(instance, clientId, scope);

                return new OkObjectResult(new
                {
                    jwtClaims = GetJwtPayload(req),
                    claimsPrincipalClaims = user.Claims.ToDictionary(x => x.Type, x => x.Value)
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }

        private static Dictionary<string, object> GetJwtPayload(HttpRequest req)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(Base64Url.Decode(req.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Split('.')[1])));
        }
    }
}
