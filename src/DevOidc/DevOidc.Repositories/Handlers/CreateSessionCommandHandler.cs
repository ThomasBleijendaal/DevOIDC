using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations;

namespace DevOidc.Repositories.Handlers
{
    public class CreateSessionCommandHandler : ICommandHandler<CreateSessionCommand>
    {
        private readonly IWriteRepository<SessionEntity> _writeRepository;

        public CreateSessionCommandHandler(IWriteRepository<SessionEntity> writeRepository)
        {
            _writeRepository = writeRepository;
        }

        public async Task HandleAsync(CreateSessionCommand command)
        {
            await _writeRepository.CreateEntityAsync(new SessionCreation(command));
        }
    }
}
