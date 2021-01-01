using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications
{
    public class GetTenantSpecification : ISpecification<TenantEntity, TenantDto>
    {
        private readonly string _tenantId;

        public GetTenantSpecification(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Func<TenantEntity, TenantDto> Projection => tenant => new TenantDto
        {
            TenantId = tenant.RowKey,
            TokenLifetime = TimeSpan.Parse(tenant.TokenLifetime ?? "00:00:01")
        };

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.RowKey == _tenantId;
    }
}
