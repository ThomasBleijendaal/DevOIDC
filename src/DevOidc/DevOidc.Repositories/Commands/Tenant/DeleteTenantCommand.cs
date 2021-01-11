using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Tenant
{
    public class DeleteTenantCommand : ICommand
    {
        public DeleteTenantCommand(string ownerName, string tenantId)
        {
            OwnerName = ownerName;
            TenantId = tenantId;
        }

        public string OwnerName { get; }
        public string TenantId { get; }
    }
}
