using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.User
{
    public class GetUsersByTenantIdSpecification : UserSpecificationBase, ISpecification<UserEntity, UserDto>
    {
        private readonly string _tenantId;

        public GetUsersByTenantIdSpecification(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user => user.PartitionKey == _tenantId;
    }
}
