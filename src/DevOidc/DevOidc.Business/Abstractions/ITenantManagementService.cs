using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface ITenantManagementService
    {
        Task<IReadOnlyList<TenantDto>> GetTenantsAsync();
        Task DeleteTenantAsync(string tenantId);
        Task<string> CreateTenantAsync(string ownerName, TenantDto tenant);
    }
}
