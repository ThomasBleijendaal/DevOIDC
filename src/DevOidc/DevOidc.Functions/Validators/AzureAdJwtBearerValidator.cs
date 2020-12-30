using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DevOidc.Functions.Validators
{
    internal class AzureAdJwtBearerValidator : IAuthenticationValidator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AzureAdJwtBearerValidator> _logger;

        public AzureAdJwtBearerValidator(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AzureAdJwtBearerValidator> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task EnsureValidUserAsync(string tenantId, string clientId, string scope)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var wellKnownEndpoint = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{tenantId}/.well-known/openid-configuration";

            var documentRetriever = new HttpDocumentRetriever()
            {
                RequireHttps = false
            };

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever(), documentRetriever);

            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeaders);
            var authorizationHeader = authorizationHeaders.ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.Contains("Bearer "))
            {
                throw new UnauthorizedAccessException();
            }

            var accessToken = authorizationHeader["Bearer ".Length..];

            try
            {
                var oidcWellknownEndpoints = await configurationManager.GetConfigurationAsync();

                var tokenValidator = new JwtSecurityTokenHandler();

                IdentityModelEventSource.ShowPII = true;

                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateAudience = true,
                    ValidAudience = scope,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                    ValidIssuer = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{tenantId}/"
                };

                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
                if (IsClaimValid("aud", scope, claimsPrincipal))
                {
                    return;
                }

                throw new UnauthorizedAccessException();
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Exception thrown during token validation.");

                throw new UnauthorizedAccessException(ex.Message);
            }
        }

        private bool IsClaimValid(string claimName, string requiredClaimValue, ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return false;
            }

            var claim = claimsPrincipal.HasClaim(x => x.Type == claimName)
                ? claimsPrincipal.Claims.First(x => x.Type == claimName).Value
                : string.Empty;

            return requiredClaimValue == claim;
        }
    }
}
