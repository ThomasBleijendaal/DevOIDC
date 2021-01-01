using System.Threading.Tasks;

namespace DevOidc.Business.Abstractions
{
    public interface ITenantManagementService
    {
        Task CreateTenantAsync(string ownerId);
    }
}
