using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Session;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Session
{
    public class DeleteSessionSelection : ISelection<SessionEntity>
    {
        private readonly DeleteSessionCommand _command;

        public DeleteSessionSelection(DeleteSessionCommand command)
        {
            _command = command;
        }

        public Expression<Func<SessionEntity, bool>> Criteria => session => session.PartitionKey == _command.TenantId && session.RowKey == _command.SessionId;
    }
}
