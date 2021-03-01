using System;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Operations.Client
{
    public class ClientCreation : ICreation<ClientEntity>
    {
        private readonly CreateClientCommand _command;

        public ClientCreation(CreateClientCommand command)
        {
            _command = command;
        }

        public string PartitionKey => _command.TenantId;

        public Action<ClientEntity> Mutation => client =>
        {
            client.RowKey = _command.Client.ClientId;
            client.AccessTokenExtraClaims = JsonConvert.SerializeObject(_command.Client.AccessTokenExtraClaims);
            client.IdTokenExtraClaims = JsonConvert.SerializeObject(_command.Client.IdTokenExtraClaims);
            client.RedirectUris = JsonConvert.SerializeObject(_command.Client.RedirectUris);
            client.Scopes = JsonConvert.SerializeObject(_command.Client.Scopes);
            client.Name = _command.Client.Name;
            client.ClientSecret = Guid.NewGuid().ToString().Replace("-", "");
        };

        public string CreatedId { set => _command.ClientId = value; }
    }
}
