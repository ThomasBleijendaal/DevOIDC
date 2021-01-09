using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.User
{
    public class DeleteUserSelection : ISelection<UserEntity>
    {
        private readonly string _userId;
        private readonly string _tenantId;

        public DeleteUserSelection(string userId, string tenantId)
        {
            _userId = userId;
            _tenantId = tenantId;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user => user.RowKey == _userId && user.PartitionKey == _tenantId;
    }
}
