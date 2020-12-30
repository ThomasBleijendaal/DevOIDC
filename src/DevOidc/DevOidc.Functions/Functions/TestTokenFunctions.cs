using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DevOidc.Functions.Abstractions;

namespace DevOidc.Functions.Functions
{
    public class TestTokenFunctions
    {
        private readonly IAuthenticationValidator _authenticationValidator;

        public TestTokenFunctions(IAuthenticationValidator authenticationValidator)
        {
            _authenticationValidator = authenticationValidator;
        }

        [FunctionName(nameof(TestOidcToken))]
        public async Task<IActionResult> TestOidcToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test/{tenantId}/{clientId}/{scope}")] HttpRequest req,
            string tenantId,
            string clientId,
            string scope,
            ILogger log)
        {
            try
            {
                await _authenticationValidator.EnsureValidUserAsync(tenantId, clientId, scope);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }
    }
}
