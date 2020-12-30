using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface ITenantService
    {
        Task<ClientDto?> GetClientAsync(string tenantId, string clientId);

        Task<UserDto?> AuthenticateUserAsync(string tenantId, string clientId, string userName, string password);
    }
}
