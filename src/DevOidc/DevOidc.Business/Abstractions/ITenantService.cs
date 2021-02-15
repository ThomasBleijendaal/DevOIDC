using System.Threading.Tasks;
using DevOidc.Core.Models.Dtos;

namespace DevOidc.Business.Abstractions
{
    public interface ITenantService
    {
        Task<TenantDto?> GetTenantAsync(string tenantId);

        Task<ClientDto?> GetClientAsync(string tenantId, string clientId);

        Task<IEncryptionProvider?> GetEncryptionProviderAsync(string tenantId);

        Task<UserDto?> AuthenticateUserAsync(string tenantId, string clientId, string userName, string password);

        Task<UserDto?> GetUserAsync(string tenantId, string clientId, string userId);
    }
}
