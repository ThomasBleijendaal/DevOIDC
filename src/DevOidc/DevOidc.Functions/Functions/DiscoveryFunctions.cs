using System;
using System.Net;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Models.Response;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
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

        [FunctionName(nameof(GetMetadataAsync))]
        public async Task<HttpResponseData> GetMetadataAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/.well-known/openid-configuration")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var tenant = await _tenantService.GetTenantAsync(tenantId);
            if (tenant == null)
            {
                return Response.NotFound();
            }

            var baseUri = context.GetBaseUri("/.well-known");

            return Response.Json(new MetadataResponseModel
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

        [FunctionName(nameof(GetKeysAsync))]
        public async Task<HttpResponseData> GetKeysAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/discovery/keys")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var baseUri = context.GetBaseUri("/discovery/keys");

            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenantId);
            if (encryptionProvider == null)
            {
                return Response.NotFound();
            }

            var key = encryptionProvider.GetPublicKey();

            return Response.Json(new KeysResponseModel
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
