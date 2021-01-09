using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Session;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.Session;

namespace DevOidc.Repositories.Handlers.Session
{
    public class DeleteSessionCommandHandler : ICommandHandler<DeleteSessionCommand>
    {
        private readonly IWriteRepository<SessionEntity> _writeRepository;

        public DeleteSessionCommandHandler(IWriteRepository<SessionEntity> writeRepository)
        {
            _writeRepository = writeRepository;
        }

        public async Task HandleAsync(DeleteSessionCommand command) 
            => await _writeRepository.DeleteEntitiesAsync(new DeleteSessionSelection(command));
    }
}
