using System;
using System.Collections.Generic;
using DevOidc.Core.Models;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Specifications.Base
{
    public class ClientSpecificationBase
    {
        public Func<ClientEntity, ClientDto> Projection => client => new ClientDto
        {
            ExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.ExtraClaims ?? "") ?? new Dictionary<string, string>(),
            Name = client.Name ?? "",
            ClientId = client.RowKey,
            TenantId = client.PartitionKey,
            RedirectUris = JsonConvert.DeserializeObject<List<string>>(client.RedirectUris ?? "") ?? new List<string>(),
            Scopes = JsonConvert.DeserializeObject<List<ScopeDto>>(client.Scopes ?? "") ?? new List<ScopeDto>()
        };
    }
}
