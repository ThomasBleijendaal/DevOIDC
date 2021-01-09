using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Client
{
    public class DeleteClientCommand : ICommand
    {
        public DeleteClientCommand(string tenantId, string clientId)
        {
            TenantId = tenantId;
            ClientId = clientId;
        }

        public string TenantId { get; }
        public string ClientId { get; }
    }
}
