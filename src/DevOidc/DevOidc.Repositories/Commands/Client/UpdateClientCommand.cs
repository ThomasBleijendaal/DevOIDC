using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Client
{
    public class UpdateClientCommand : ICommand
    {
        public UpdateClientCommand(string tenantId, string clientId, ClientDto client)
        {
            TenantId = tenantId;
            ClientId = clientId;
            Client = client;
        }

        public string TenantId { get; }
        public string ClientId { get; }
        public ClientDto Client { get; }
    }
}
