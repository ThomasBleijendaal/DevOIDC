using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Client
{
    public class GetClientByIdSpecification : ClientSpecificationBase, ISpecification<ClientEntity, ClientDto>
    {
        private readonly string _tenantId;
        private readonly string _clientId;

        public GetClientByIdSpecification(string tenantId, string clientId)
        {
            _tenantId = tenantId;
            _clientId = clientId;
        }

        public Expression<Func<ClientEntity, bool>> Criteria => client => client.PartitionKey == _tenantId && client.RowKey == _clientId;
    }
}
