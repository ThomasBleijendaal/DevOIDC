using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Tenant
{
    public class DeleteTenantSelection : ISelection<TenantEntity>
    {
        private readonly string _tenantId;
        private readonly string _ownerName;

        public DeleteTenantSelection(DeleteTenantCommand command)
        {
            _tenantId = command.TenantId;
            _ownerName = command.OwnerName;
        }
        public DeleteTenantSelection(TenantDto tenant)
        {
            _tenantId = tenant.TenantId;
            _ownerName = tenant.OwnerName;
        }

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.PartitionKey == _ownerName && tenant.RowKey == _tenantId;
    }
}
