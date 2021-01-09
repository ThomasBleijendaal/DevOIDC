using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Client;

namespace DevOidc.Repositories.Handlers.Client
{
    public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
    {
        private readonly IWriteRepository<ClientEntity> _clientRepository;

        public DeleteClientCommandHandler(IWriteRepository<ClientEntity> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task HandleAsync(DeleteClientCommand command) 
            => await _clientRepository.DeleteEntitiesAsync(new DeleteClientsOfTenantSelection(command));
    }
}
