using DevOidc.Functions.Models;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DevOidc.Functions.Functions
{
    public class DiscoveryFunctions
    {
        public const string BaseUrl = "http://localhost:7071/";
        private readonly IJwtService _jwtService;

        public DiscoveryFunctions(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [FunctionName(nameof(GetMetadata))]
        public IActionResult GetMetadata(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/.well-known/openid-configuration")] HttpRequest req,
            string tenantId,
            ILogger log)
        {
            return new OkObjectResult(new MetadataResponseModel
            {
                TokenEndpoint = $"{BaseUrl}{tenantId}/token",
                TokenEndpointAuthMethodsSupported = new[] { "private_key_jwt" },
                JwksUri = $"{BaseUrl}{tenantId}/discovery/keys",
                ResponseModesSupported = new[] { "query", "fragment" },
                SubjectTypesSupported = new[] { "pairwise" },
                IdTokenSigningAlgValuesSupported = new[] { "RS256" },
                ResponseTypesSupported = new[] { "code" },
                ScopesSupported = new[] { "openid", "offline_access" },
                Issuer = $"{BaseUrl}{tenantId}",
                AuthorizationEndpoint = $"{BaseUrl}{tenantId}/authorize",
                ClaimsSupported = new[] { "sub", "iss", "aud", "exp", "email" },
                TenantRegionScope = "EU"
            });
        }

        [FunctionName(nameof(GetKeys))]
        public IActionResult GetKeys(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/discovery/keys")] HttpRequest req,
            string tenantId,
            ILogger log)
        {
            return new OkObjectResult(new KeysResponseModel
            {
                Keys = new[]
                {
                    new KeyResponseModel
                    {
                        Kty = "RSA",
                        Alg = "RS256",
                        Use = "sig",
                        Kid = "default-kid", // TODO
                        //X5t = "default-kid", // TODO
                        N = _jwtService.GetPublicKey(), // TODO
                        E = "AQAB", // TODO
                        //X5c = new [] { _jwtService.GetPublicKey() }, // TODO
                        Issuer = $"{BaseUrl}{tenantId}" // TODO
                    }
                }
            });
        }
    }
}
