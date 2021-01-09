using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.User;

namespace DevOidc.Business.Tenant
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IReadRepository<UserEntity> _readRepository;
        private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;
        private readonly ICommandHandler<UpdateUserCommand> _updateUserCommandHandler;
        private readonly ICommandHandler<DeleteUserCommand> _deleteUserCommandHandler;

        public UserManagementService(
            IReadRepository<UserEntity> readRepository,
            ICommandHandler<CreateUserCommand> createUserCommandHandler,
            ICommandHandler<UpdateUserCommand> updateUserCommandHandler,
            ICommandHandler<DeleteUserCommand> deleteUserCommandHandler)
        {
            _readRepository = readRepository;
            _createUserCommandHandler = createUserCommandHandler;
            _updateUserCommandHandler = updateUserCommandHandler;
            _deleteUserCommandHandler = deleteUserCommandHandler;
        }

        public async Task<string> CreateUserAsync(string tenantId, UserDto user)
        {
            var command = new CreateUserCommand(tenantId, user);

            await _createUserCommandHandler.HandleAsync(command);

            return command.UserId ?? throw new InvalidOperationException();
        }

        public async Task DeleteUserAsync(string tenantId, string userId)
            => await _deleteUserCommandHandler.HandleAsync(new DeleteUserCommand(tenantId, userId));

        public async Task<IReadOnlyList<UserDto>> GetAllUsersAsync(string tenantId)
            => await _readRepository.GetListAsync(new GetUsersByTenantIdSpecification(tenantId));

        public async Task<UserDto?> GetUserByIdAsync(string tenantId, string userId)
            => await _readRepository.GetAsync(new GetUserByIdSpecification(tenantId, userId));

        public async Task UpdateUserAsync(string tenantId, string userId, UserDto user)
            => await _updateUserCommandHandler.HandleAsync(new UpdateUserCommand(tenantId, userId, user));
    }
}
