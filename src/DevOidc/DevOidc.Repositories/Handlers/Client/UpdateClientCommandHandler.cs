using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Client;

namespace DevOidc.Repositories.Handlers.Client
{
    public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand>
    {
        private readonly IWriteRepository<ClientEntity> _clientRepository;

        public UpdateClientCommandHandler(IWriteRepository<ClientEntity> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task HandleAsync(UpdateClientCommand command)
            => await _clientRepository.UpdateSingleEntityAsync(new UpdateClientOperation(command));
    }
}
