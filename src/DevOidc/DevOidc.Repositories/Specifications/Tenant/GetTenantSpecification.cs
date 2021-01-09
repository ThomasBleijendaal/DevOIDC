using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Tenant
{
    public class GetTenantSpecification : TenantSpecificationBase, ISpecification<TenantEntity, TenantDto>
    {
        private readonly string _tenantId;

        public GetTenantSpecification(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.RowKey == _tenantId;
    }
}
