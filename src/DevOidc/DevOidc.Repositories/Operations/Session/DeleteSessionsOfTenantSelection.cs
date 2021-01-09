using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Session
{
    public class DeleteSessionsOfTenantSelection : ISelection<SessionEntity>
    {
        private readonly DeleteTenantCommand _command;

        public DeleteSessionsOfTenantSelection(DeleteTenantCommand command)
        {
            _command = command;
        }

        public Expression<Func<SessionEntity, bool>> Criteria => session => session.PartitionKey == _command.TenantId;
    }
}
