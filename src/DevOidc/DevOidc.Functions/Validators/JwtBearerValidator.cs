using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Validators
{
    internal class JwtBearerValidator : IAuthenticationValidator
    {
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;
        private readonly ILogger<JwtBearerValidator> _logger;

        public JwtBearerValidator(
            IFunctionExecutionContextAccessor functionExecutionContextAccessor,
            ILogger<JwtBearerValidator> logger)
        {
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> GetClaimsAysnc(Uri instanceUri)
        {
            var configurationManager = BuildConfigurationManager(instanceUri);
            var accessToken = GetAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No access token provided.");
            }

            try
            {
                IdentityModelEventSource.ShowPII = true;

                var oidcWellknownEndpoints = await configurationManager.GetConfigurationAsync();

                var tokenValidator = new JwtSecurityTokenHandler
                {
                    MapInboundClaims = false
                };

                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                    ValidIssuer = $"{instanceUri}"
                };

                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
                return claimsPrincipal;
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Exception thrown during token validation.");

                throw new UnauthorizedAccessException(ex.Message);
            }
        }

        public async Task<ClaimsPrincipal> GetValidUserAsync(Uri instanceUri, string clientId, string scope, Uri? validIssuer = default)
        {
            var configurationManager = BuildConfigurationManager(instanceUri);
            var accessToken = GetAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No access token provided.");
            }

            try
            {
                IdentityModelEventSource.ShowPII = true;

                var oidcWellknownEndpoints = await configurationManager.GetConfigurationAsync();

                var tokenValidator = new JwtSecurityTokenHandler
                {
                    MapInboundClaims = instanceUri.AbsoluteUri.Contains("microsoft")
                };

                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateAudience = true,
                    ValidAudience = scope,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                    ValidIssuer = $"{validIssuer ?? instanceUri}"
                };

                return tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Exception thrown during token validation.");

                throw new UnauthorizedAccessException(ex.Message);
            }
        }

        private static ConfigurationManager<OpenIdConnectConfiguration> BuildConfigurationManager(Uri instanceUri)
        {
            var wellKnownEndpoint = $"{instanceUri}/.well-known/openid-configuration";

            var documentRetriever = new HttpDocumentRetriever()
            {
                RequireHttps = instanceUri.Scheme == "https"
            };

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever(), documentRetriever);
            return configurationManager;
        }

        private string? GetAccessToken()
        {
            if (_functionExecutionContextAccessor.FunctionExecutionContext?.InvocationRequest is InvocationRequest invocation)
            {
                var req = invocation.InputData.FirstOrDefault(x => x.Name == "req");
                if (req?.Data?.Http != null)
                {
                    try
                    {
                        var request = new Microsoft.Azure.Functions.Worker.HttpRequestData(req.Data.Http);

                        var authorizationHeader = request.Headers.FirstOrDefault(x => x.Key.Equals("authorization", StringComparison.InvariantCultureIgnoreCase));
                        if (authorizationHeader.Value is string headerValue)
                        {
                            return headerValue.Replace("Bearer ", "");
                        }
                    }
                    catch { }
                }
            }

            return default;
        }
    }
}
