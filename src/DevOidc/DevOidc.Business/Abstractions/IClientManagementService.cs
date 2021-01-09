using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface IClientManagementService
    {
        Task<ClientDto?> GetClientByIdAsync(string tenantId, string clientId);
        Task<IReadOnlyList<ClientDto>> GetAllClientsAsync(string tenantId);
        Task DeleteClientAsync(string tenantId, string clientId);
        Task<string> CreateClientAsync(string tenantId, ClientDto client);
        Task UpdateClientAsync(string tenantId, string clientId, ClientDto client);
    }
}
