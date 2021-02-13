using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DevOidc.Functions.Validators
{
    internal class JwtBearerValidator : IAuthenticationValidator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtBearerValidator> _logger;

        public JwtBearerValidator(
            IHttpContextAccessor httpContextAccessor,
            ILogger<JwtBearerValidator> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> GetClaimsAysnc(Uri instanceUri)
        {
            var configurationManager = BuildConfigurationManager(instanceUri);
            var accessToken = GetAccessToken();

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

        private string GetAccessToken()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeaders);
            var authorizationHeader = authorizationHeaders.ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.Contains("Bearer "))
            {
                throw new UnauthorizedAccessException();
            }

            var accessToken = authorizationHeader["Bearer ".Length..];
            return accessToken;
        }
    }
}
