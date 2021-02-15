using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models.Dtos;

namespace DevOidc.Business.Abstractions
{
    public interface ITenantManagementService
    {
        Task<IReadOnlyList<TenantDto>> GetTenantsAsync(string ownerName);
        Task<IReadOnlyList<TenantDto>> GetTenantsOfOthersAsync(string ownerName);
        Task DeleteTenantAsync(string ownerName, string tenantId);
        Task<string> CreateTenantAsync(string ownerName, TenantDto tenant);
        Task ClaimTenantAsync(string ownerName, string tenantId);
    }
}
