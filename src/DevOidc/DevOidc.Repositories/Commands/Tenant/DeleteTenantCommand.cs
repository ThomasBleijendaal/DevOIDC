using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Tenant
{
    public class DeleteTenantCommand : ICommand
    {
        public DeleteTenantCommand(string tenantId)
        {
            TenantId = tenantId;
        }

        public string TenantId { get; }
    }
}
