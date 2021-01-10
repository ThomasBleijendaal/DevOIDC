using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevOidc.Functions.Abstractions
{
    public interface IAuthenticationValidator
    {
        Task<ClaimsPrincipal> GetValidUserAsync(Uri instanceUri, string clientId, string scope, Uri? validIssuer = default);
    }
}
