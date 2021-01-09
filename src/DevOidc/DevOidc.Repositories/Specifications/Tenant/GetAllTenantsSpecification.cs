using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Tenant
{
    public class GetAllTenantsSpecification : TenantSpecificationBase, ISpecification<TenantEntity, TenantDto>
    {
        public Expression<Func<TenantEntity, bool>> Criteria => tenant => true;
    }
}
