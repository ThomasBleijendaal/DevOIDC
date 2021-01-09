using System.Threading.Tasks;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Operations.User;

namespace DevOidc.Repositories.Handlers.User
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IWriteRepository<UserEntity> _userRepository;

        public CreateUserCommandHandler(IWriteRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(CreateUserCommand command)
            => await _userRepository.CreateEntityAsync(new UserCreation(command));
    }
}
