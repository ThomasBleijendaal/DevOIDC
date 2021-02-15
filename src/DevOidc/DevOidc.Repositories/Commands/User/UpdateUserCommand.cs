using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.User
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(string tenantId, string userId, UserDto user, bool updatePassword = false)
        {
            TenantId = tenantId;
            UserId = userId;
            User = user;
            UpdatePassword = updatePassword;
        }

        public string TenantId { get; }
        public string UserId { get; }
        public UserDto User { get; }

        public bool UpdatePassword { get; }
    }
}
