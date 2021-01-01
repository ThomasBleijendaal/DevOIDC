using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Specifications
{
    public class GetClientSpecification : ISpecification<ClientEntity, ClientDto>
    {
        private readonly string _tenantId;
        private readonly string _clientId;

        public GetClientSpecification(string tenantId, string clientId)
        {
            _tenantId = tenantId;
            _clientId = clientId;
        }

        public Func<ClientEntity, ClientDto> Projection => client => new ClientDto
        {
            ClientId = client.RowKey,
            ExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.ExtraClaims ?? "") ?? new Dictionary<string, string>(),
            RedirectUris = JsonConvert.DeserializeObject<List<string>>(client.RedirectUris ?? "") ?? new List<string>(),
            Scopes = JsonConvert.DeserializeObject<List<ScopeDto>>(client.Scopes ?? "") ?? new List<ScopeDto>(),
            TenantId = client.PartitionKey
        };

        public Expression<Func<ClientEntity, bool>> Criteria => client => client.PartitionKey == _tenantId && client.RowKey == _clientId;
    }
}
