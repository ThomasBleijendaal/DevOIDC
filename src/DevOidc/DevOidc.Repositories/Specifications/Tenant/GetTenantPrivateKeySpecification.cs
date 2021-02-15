using System;
using System.Linq.Expressions;
using System.Text;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications.Tenant
{
    public class GetTenantPrivateKeySpecification : ISpecification<TenantEntity, PrivateKeyDto>
    {
        private readonly string _tenantId;

        public GetTenantPrivateKeySpecification(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Func<TenantEntity, PrivateKeyDto> Projection => tenant => new PrivateKeyDto
        {
            PrivateKey = Encoding.UTF8.GetString(Convert.FromBase64String(tenant.PrivateKey)),
            PublicKey = Encoding.UTF8.GetString(Convert.FromBase64String(tenant.PublicKey))
        };

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.RowKey == _tenantId;
    }


}
