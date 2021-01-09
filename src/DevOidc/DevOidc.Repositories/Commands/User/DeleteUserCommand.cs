using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.User
{
    public class DeleteUserCommand : ICommand
    {
        public DeleteUserCommand(string tenantId, string userId)
        {
            TenantId = tenantId;
            UserId = userId;
        }

        public string TenantId { get; }
        public string UserId { get; }
    }
}
