using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.User
{
    public class DeleteUsersOfTenantSelection : ISelection<UserEntity>
    {
        private readonly string _tenantId;
        private readonly string? _userId;

        public DeleteUsersOfTenantSelection(DeleteTenantCommand command)
        {
            _tenantId = command.TenantId;
        }
        public DeleteUsersOfTenantSelection(DeleteUserCommand command)
        {
            _tenantId = command.TenantId;
            _userId = command.UserId;
        }

        public Expression<Func<UserEntity, bool>> Criteria => _userId == null
            ? user => user.PartitionKey == _tenantId
            : user => user.PartitionKey == _tenantId && user.RowKey == _userId;
    }
}
