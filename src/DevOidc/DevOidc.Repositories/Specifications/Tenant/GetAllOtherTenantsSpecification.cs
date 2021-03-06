﻿using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.Tenant
{
    public class GetAllOtherTenantsSpecification : TenantSpecificationBase, ISpecification<TenantEntity, TenantDto>
    {
        private readonly string _ownerName;

        public GetAllOtherTenantsSpecification(string ownerName)
        {
            _ownerName = ownerName;
        }

        public Expression<Func<TenantEntity, bool>> Criteria => tenant => tenant.PartitionKey != _ownerName;
    }
}
