using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Client;

namespace DevOidc.Repositories.Handlers.Client
{
    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand>
    {
        private readonly IWriteRepository<ClientEntity> _clientRepository;

        public CreateClientCommandHandler(IWriteRepository<ClientEntity> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task HandleAsync(CreateClientCommand command)
            => await _clientRepository.CreateEntityAsync(new ClientCreation(command));
    }
}
