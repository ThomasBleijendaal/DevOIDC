using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Tenant;

namespace DevOidc.Repositories.Handlers.Tenant
{
    public class ClaimTenantCommandHandler : ICommandHandler<ClaimTenantCommand>
    {
        private readonly IWriteRepository<TenantEntity> _tenantRepository;

        public ClaimTenantCommandHandler(IWriteRepository<TenantEntity> tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task HandleAsync(ClaimTenantCommand command) 
            => await _tenantRepository.ReinsertEntityAsync(new ClaimTenantOperation(command));
    }
}
