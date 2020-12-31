using System.Threading.Tasks;
using DevOidc.Core.Extensions;
using DevOidc.Functions.Models.Response;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class DiscoveryFunctions
    {
        private readonly ITenantService _tenantService;

        public DiscoveryFunctions(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [FunctionName(nameof(GetMetadata))]
        public IActionResult GetMetadata(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/.well-known/openid-configuration")] HttpRequest req,
            string tenantId)
        {
            return new OkObjectResult(new MetadataResponseModel
            {
                TokenEndpoint = $"{req.HttpContext.GetServerBaseUri()}{tenantId}/token",
                TokenEndpointAuthMethodsSupported = new[] { "private_key_jwt" },
                JwksUri = $"{req.HttpContext.GetServerBaseUri()}{tenantId}/discovery/keys",
                ResponseModesSupported = new[] { "query", "fragment" },
                SubjectTypesSupported = new[] { "pairwise" },
                IdTokenSigningAlgValuesSupported = new[] { "RS256" },
                ResponseTypesSupported = new[] { "code" },
                ScopesSupported = new[] { "openid", "offline_access" },
                Issuer = $"{req.HttpContext.GetServerBaseUri()}{tenantId}",
                AuthorizationEndpoint = $"{req.HttpContext.GetServerBaseUri()}{tenantId}/authorize",
                ClaimsSupported = new[] { "sub", "iss", "aud", "exp", "email" },
                TenantRegionScope = "EU"
            });
        }

        [FunctionName(nameof(GetKeysAsync))]
        public async Task<IActionResult> GetKeysAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/discovery/keys")] HttpRequest req,
            string tenantId)
        {
            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return new NotFoundResult();
            }

            var key = encryptionProvider.GetPublicKey();

            return new OkObjectResult(new KeysResponseModel
            {
                Keys = new[]
                {
                    new KeyResponseModel
                    {
                        Kty = key.KeyType,
                        Alg = key.Algorithm,
                        Use = key.Use,
                        Kid = key.Id,
                        N = key.Modulus,
                        E = key.Exponent,
                        Issuer = $"{req.HttpContext.GetServerBaseUri()}{tenantId}"
                    }
                }
            });
        }
    }
}
