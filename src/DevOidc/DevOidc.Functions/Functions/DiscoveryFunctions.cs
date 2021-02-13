using System;
using System.Net;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Models.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace DevOidc.Functions.Functions
{
    public class DiscoveryFunctions
    {
        private readonly ITenantService _tenantService;

        public DiscoveryFunctions(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [FunctionName(nameof(GetMetadataAsync))]
        public async Task<HttpResponseData> GetMetadataAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/.well-known/openid-configuration")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }

            var baseUri = GetBaseUri(context, ".well-known");

            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(new MetadataResponseModel
            {
                TokenEndpoint = $"{baseUri}{tenantId}/token",
                TokenEndpointAuthMethodsSupported = new[] { "client_secret_post", "private_key_jwt", "client_secret_basic" },
                JwksUri = $"{baseUri}{tenantId}/discovery/keys",
                ResponseModesSupported = new[] { "fragment", "form_post", "query" },
                SubjectTypesSupported = new[] { "pairwise" },
                IdTokenSigningAlgValuesSupported = new[] { "RS256" },
                ResponseTypesSupported = new[] { "code", "id_token", "code id_token", "token id_token", "token" },
                ScopesSupported = new[] { "openid" },
                Issuer = $"{baseUri}{tenantId}",
                AuthorizationEndpoint = $"{baseUri}{tenantId}/authorize",
                ClaimsSupported = new[] { "sub", "iss", "aud", "exp", "email" },
                TenantRegionScope = "EU",
                EndSessionEndpoint = $"{baseUri}{tenantId}/logout",
                HttpLogoutSupported = true,
                CheckSessionIframe = $"{baseUri}{tenantId}/checksession",
                UserinfoEndpoint = $"{baseUri}{tenantId}/oidc/userinfo",
            }));
        }

        [FunctionName(nameof(GetKeysAsync))]
        public async Task<HttpResponseData> GetKeysAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/discovery/keys")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }

            var baseUri = GetBaseUri(context, "/discovery/keys");

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }

            var key = encryptionProvider.GetPublicKey();

            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(new KeysResponseModel
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
            }));
        }

        private static string GetBaseUri(FunctionExecutionContext context, string readTo)
        {
            var requestUri = new Uri(context.InvocationRequest.TriggerMetadata["req"].Http.Url);
            var baseUri = requestUri.ToString().Split(readTo)[0];
            return baseUri;
        }
    }
}
