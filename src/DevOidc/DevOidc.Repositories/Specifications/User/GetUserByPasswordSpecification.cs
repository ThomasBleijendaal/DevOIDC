using System;
using System.Linq.Expressions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Base;

namespace DevOidc.Repositories.Specifications.User
{
    public class GetUserByPasswordSpecification : UserSpecificationBase, ISpecification<UserEntity, UserDto>
    {
        private readonly string _tenantId;
        private readonly string _userName;
        private readonly string _password;

        public GetUserByPasswordSpecification(string tenantId, string userName, string password)
        {
            _tenantId = tenantId;
            _userName = userName;
            _password = password;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user =>
            user.PartitionKey == _tenantId &&
            user.UserName == _userName &&
            user.Password == _password;
    }
}
