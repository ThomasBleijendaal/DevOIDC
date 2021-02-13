using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs.Script.Grpc.Messages;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Authentication
{
    public class AuthenticationMiddleware
    {
        private readonly AzureAdConfig _authenticationConfig;
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;

        public AuthenticationMiddleware(
            IOptions<AzureAdConfig> authenticationConfig,
            IFunctionExecutionContextAccessor functionExecutionContextAccessor)
        {
            _authenticationConfig = authenticationConfig.Value;
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        public async Task InvokeAsync(FunctionExecutionContext context, FunctionExecutionDelegate next)
        {
            if (context.InvocationRequest is InvocationRequest invocation)
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
                            var user = await GetValidUserAsync(headerValue);
                            if (user != null)
                            {
                                context.Items.Add("User", user);
                            }
                        }
                    }
                    catch { }

                    await next(context);
                    return;
                }
            }

            await next(context);
        }

        public async Task<ClaimsPrincipal> GetValidUserAsync(string? authorizationHeader)
        {
            var configurationManager = BuildConfigurationManager(_authenticationConfig.Authority);
            var accessToken = GetAccessToken(authorizationHeader);

            try
            {
                // TODO: remove
                IdentityModelEventSource.ShowPII = true;

                var oidcWellknownEndpoints = await configurationManager.GetConfigurationAsync();

                var tokenValidator = new JwtSecurityTokenHandler
                {
                    MapInboundClaims = _authenticationConfig.Authority.AbsoluteUri.Contains("microsoft")
                };

                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidateAudience = true,
                    ValidAudience = _authenticationConfig.ValidAudience,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                    ValidIssuer = _authenticationConfig.Issuer
                };

                return tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
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

            return new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever(), documentRetriever);
        }

        private static string GetAccessToken(string? authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.Contains("Bearer "))
            {
                throw new UnauthorizedAccessException();
            }

            var accessToken = authorizationHeader["Bearer ".Length..];
            return accessToken;
        }
    }
}
