using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Models;
using Microsoft.Extensions.Options;

namespace DevOidc.Functions.Functions.Base
{
    public abstract class BaseAdAuthenticatedFunctions
    {
        private readonly AzureAdConfig _config;
        private readonly IAuthenticationValidator _authenticationValidator;

        protected BaseAdAuthenticatedFunctions(
            IOptions<AzureAdConfig> options,
            IAuthenticationValidator authenticationValidator)
        {
            _config = options.Value;
            _authenticationValidator = authenticationValidator;
        }

        protected async Task<ClaimsPrincipal> GetValidUserAsync()
            => await _authenticationValidator.GetValidUserAsync(
                new Uri(new Uri(_config.Instance), _config.TenantId),
                _config.ClientId,
                _config.ValidAudience,
                new Uri(new Uri(_config.Issuer), $"{_config.TenantId}/"));
    }
}
