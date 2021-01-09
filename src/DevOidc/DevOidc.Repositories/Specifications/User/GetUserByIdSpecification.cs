using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.User
{
    public class GetUserByIdSpecification : UserSpecificationBase, ISpecification<UserEntity, UserDto>
    {
        private readonly string _tenantId;
        private readonly string _userId;

        public GetUserByIdSpecification(string tenantId, string userId)
        {
            _tenantId = tenantId;
            _userId = userId;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user =>
            user.PartitionKey == _tenantId &&
            user.RowKey == _userId;
    }
}
