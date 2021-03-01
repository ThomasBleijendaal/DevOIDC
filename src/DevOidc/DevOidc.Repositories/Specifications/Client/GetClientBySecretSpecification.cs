using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Client
{
    public class GetClientBySecretSpecification : ClientSpecificationBase, ISpecification<ClientEntity, ClientDto>
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public GetClientBySecretSpecification(string tenantId, string clientId, string clientSecret)
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public Expression<Func<ClientEntity, bool>> Criteria => client => client.PartitionKey == _tenantId && client.RowKey == _clientId && client.ClientSecret == _clientSecret;
    }
}
