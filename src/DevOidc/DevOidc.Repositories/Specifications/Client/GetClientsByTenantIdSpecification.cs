using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Client
{
    public class GetClientsByTenantIdSpecification : ClientSpecificationBase, ISpecification<ClientEntity, ClientDto>
    {
        private readonly string _tenantId;

        public GetClientsByTenantIdSpecification(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Expression<Func<ClientEntity, bool>> Criteria => user => user.PartitionKey == _tenantId;
    }
}
