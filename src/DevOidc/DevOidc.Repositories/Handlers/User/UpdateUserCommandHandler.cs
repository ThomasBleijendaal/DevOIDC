using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.User;

namespace DevOidc.Repositories.Handlers.User
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IWriteRepository<UserEntity> _userRepository;

        public UpdateUserCommandHandler(IWriteRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(UpdateUserCommand command)
            => await _userRepository.UpdateSingleEntityAsync(new UpdateUserOperation(command));
    }
}
