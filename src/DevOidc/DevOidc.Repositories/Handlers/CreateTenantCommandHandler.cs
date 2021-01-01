using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations;

namespace DevOidc.Repositories.Handlers
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
