using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Client
{
    public class CreateClientCommand : ICommand
    {
        public CreateClientCommand(string tenantId, ClientDto client)
        {
            TenantId = tenantId;
            Client = client;
        }

        public string TenantId { get; }
        public ClientDto Client { get; }

        public string? ClientId { get; internal set; }
    }
}
