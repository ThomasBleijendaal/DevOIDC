using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.User
{
    public class CreateUserCommand : ICommand
    {
        public CreateUserCommand(string tenantId, UserDto user)
        {
            TenantId = tenantId;
            User = user;
        }

        public string TenantId { get; }
        public UserDto User { get; }

        public string? UserId { get; internal set; }
    }
}
