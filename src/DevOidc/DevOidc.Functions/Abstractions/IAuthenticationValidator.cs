using System.Threading.Tasks;

namespace DevOidc.Functions.Abstractions
{
    public interface IAuthenticationValidator
    {
        Task EnsureValidUserAsync(string tenantId, string clientId, string scope);
    }
}
