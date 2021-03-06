﻿using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Operations.Client
{
    public class UpdateClientOperation : IOperation<ClientEntity>
    {
        private readonly UpdateClientCommand _command;

        public UpdateClientOperation(UpdateClientCommand command)
        {
            _command = command;
        }

        public Action<ClientEntity> Mutation => client =>
        {
            client.AccessTokenExtraClaims = JsonConvert.SerializeObject(_command.Client.AccessTokenExtraClaims);
            client.IdTokenExtraClaims = JsonConvert.SerializeObject(_command.Client.IdTokenExtraClaims);
            client.RedirectUris = JsonConvert.SerializeObject(_command.Client.RedirectUris);
            client.Scopes = JsonConvert.SerializeObject(_command.Client.Scopes);
            client.Name = _command.Client.Name;
            client.ClientSecret = _command.Client.ClientSecret;
        };

        public Expression<Func<ClientEntity, bool>> Criteria => client =>
            client.PartitionKey == _command.TenantId &&
            client.RowKey == _command.ClientId;
    }
}
