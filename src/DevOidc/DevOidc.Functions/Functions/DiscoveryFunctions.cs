using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Models.Response;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DevOidc.Functions.Functions
{
    public class DiscoveryFunctions
    {
        private readonly ITenantService _tenantService;

        public DiscoveryFunctions(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [Function(nameof(GetMetadataAsync))]
        [AllowAnonymous]
        public async Task<HttpResponseData> GetMetadataAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/.well-known/openid-configuration")] HttpRequestData req, 
            string tenantId, 
            FunctionContext context)
        {   
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                return req.CreateNotFoundResponse();
            }

            var baseUri = context.GetBaseUri("/.well-known");

            return req.CreateJsonResponse(new MetadataResponseModel
            {
                TokenEndpoint = $"{baseUri}/token",
                TokenEndpointAuthMethodsSupported = new[] { "client_secret_post", "private_key_jwt", "client_secret_basic" },
                JwksUri = $"{baseUri}/discovery/keys",
                ResponseModesSupported = new[] { "fragment", "form_post", "query" },
                SubjectTypesSupported = new[] { "pairwise" },
                IdTokenSigningAlgValuesSupported = new[] { "RS256" },
                ResponseTypesSupported = new[] { "code", "id_token", "code id_token", "token id_token", "token" },
                ScopesSupported = new[] { "openid" },
                Issuer = baseUri,
                AuthorizationEndpoint = $"{baseUri}/authorize",
                ClaimsSupported = new[] { "sub", "iss", "aud", "exp", "email" },
                TenantRegionScope = "EU",
                EndSessionEndpoint = $"{baseUri}/logout",
                HttpLogoutSupported = true,
                CheckSessionIframe = $"{baseUri}/checksession",
                UserinfoEndpoint = $"{baseUri}/oidc/userinfo",
            });
        }

        [Function(nameof(GetKeysAsync))]
        [AllowAnonymous]
        public async Task<HttpResponseData> GetKeysAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/discovery/keys")] HttpRequestData req, 
            string tenantId, 
            FunctionContext context)
        {
            var baseUri = context.GetBaseUri("/discovery/keys");

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return req.CreateNotFoundResponse();
            }

            var key = encryptionProvider.GetPublicKey();

            return req.CreateJsonResponse(new KeysResponseModel
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
                        Issuer = baseUri
                    }
                }
            });
        }
    }
}
