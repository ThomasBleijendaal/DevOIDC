using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Tenant;

namespace DevOidc.Repositories.Handlers.Tenant
{
    public class CreateTenantCommandHandler : ICommandHandler<CreateTenantCommand>
    {
        private readonly IWriteRepository<TenantEntity> _tenantRepository;

        public CreateTenantCommandHandler(
            IWriteRepository<TenantEntity> tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task HandleAsync(CreateTenantCommand command)
            => await _tenantRepository.CreateEntityAsync(new TenantCreation(command));
    }
}
