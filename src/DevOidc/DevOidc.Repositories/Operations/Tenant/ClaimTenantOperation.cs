using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Tenant
{
    public class ClaimTenantOperation : IOperation<TenantEntity>
    {
        private readonly ClaimTenantCommand _command;

        public ClaimTenantOperation(ClaimTenantCommand command)
        {
            _command = command;
        }

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.PartitionKey != _command.OwnerName && tenant.RowKey == _command.TenantId;

        public Action<TenantEntity> Mutation => tenant =>
        {
            tenant.PartitionKey = _command.OwnerName;
        };
    }
}
