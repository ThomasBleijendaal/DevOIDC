using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Client;
using DevOidc.Repositories.Operations.Session;
using DevOidc.Repositories.Operations.Tenant;
using DevOidc.Repositories.Operations.User;

namespace DevOidc.Repositories.Handlers.Tenant
{
    public class DeleteTenantCommandHandler : ICommandHandler<DeleteTenantCommand>
    {
        private readonly IWriteRepository<TenantEntity> _tenantRepository;
        private readonly IWriteRepository<ClientEntity> _clientRepository;
        private readonly IWriteRepository<UserEntity> _userRepository;
        private readonly IWriteRepository<SessionEntity> _sessionRepository;

        public DeleteTenantCommandHandler(
            IWriteRepository<TenantEntity> tenantRepository,
            IWriteRepository<ClientEntity> clientRepository,
            IWriteRepository<UserEntity> userRepository,
            IWriteRepository<SessionEntity> sessionRepository)
        {
            _tenantRepository = tenantRepository;
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task HandleAsync(DeleteTenantCommand command)
            => await Task.WhenAll(
                _sessionRepository.DeleteEntitiesAsync(new DeleteSessionsOfTenantSelection(command)),
                _userRepository.DeleteEntitiesAsync(new DeleteUsersOfTenantSelection(command)),
                _clientRepository.DeleteEntitiesAsync(new DeleteClientsOfTenantSelection(command)),
                _tenantRepository.DeleteEntitiesAsync(new DeleteTenantSelection(command)));
    }
}
