using System;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications.Base
{
    public class TenantSpecificationBase
    {

        public Func<TenantEntity, TenantDto> Projection => tenant => new TenantDto
        {
            TenantId = tenant.RowKey,
            TokenLifetime = TimeSpan.Parse(tenant.TokenLifetime ?? "00:00:01"),
            Description = tenant.Description ?? "",
            Name = tenant.Name ?? "",
            OwnerName = tenant.PartitionKey
        };
    }
}
