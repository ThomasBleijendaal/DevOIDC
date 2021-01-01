using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications
{
    public class GetUserByIdSpecification : UserSpecificationBase, ISpecification<UserEntity, UserDto>
    {
        private readonly string _tenantId;
        private readonly string _userId;
        private readonly string _clientId;

        public GetUserByIdSpecification(string tenantId, string userId, string clientId)
        {
            _tenantId = tenantId;
            _userId = userId;
            _clientId = clientId;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user => 
            user.PartitionKey == _tenantId && 
            user.RowKey == _userId &&
            user.Clients!.CompareTo(_clientId) >= 0;
    }
}
