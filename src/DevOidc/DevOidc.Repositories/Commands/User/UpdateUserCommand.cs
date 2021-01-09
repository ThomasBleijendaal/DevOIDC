using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.User
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(string tenantId, string userId, UserDto user)
        {
            TenantId = tenantId;
            UserId = userId;
            User = user;
        }

        public string TenantId { get; }
        public string UserId { get; }
        public UserDto User { get; }

        public bool UpdatePassword { get; }
    }
}
