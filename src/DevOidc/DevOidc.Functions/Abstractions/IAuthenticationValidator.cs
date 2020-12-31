using System.Security.Claims;
using System.Threading.Tasks;

namespace DevOidc.Functions.Abstractions
{
    public interface IAuthenticationValidator
    {
        Task<ClaimsPrincipal> GetValidUserAsync(string tenantId, string clientId, string scope);
    }
}
