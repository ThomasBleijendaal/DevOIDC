using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands
{
    public class DeleteSessionCommand : ICommand
    {
        public DeleteSessionCommand(string tenantId, string sessionId)
        {
            TenantId = tenantId;
            SessionId = sessionId;
        }

        public string TenantId { get; }
        public string SessionId { get; }
    }
}
