using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.User;

namespace DevOidc.Repositories.Handlers.User
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IWriteRepository<UserEntity> _userRepository;

        public DeleteUserCommandHandler(IWriteRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(DeleteUserCommand command) 
            => await _userRepository.DeleteEntitiesAsync(new DeleteUsersOfTenantSelection(command));
    }
}
