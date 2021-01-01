using System;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations
{
    public class SessionCreation : ICreation<SessionEntity>
    {
        private readonly CreateSessionCommand _command;

        public SessionCreation(CreateSessionCommand command)
        {
            _command = command;
        }

        public string OwnerId => _command.TenantId;

        public Action<SessionEntity> Mutation => session =>
        {
            session.ClientId = _command.Client.ClientId;
            session.ScopeId = _command.Scope.ScopeId;
            session.UserId = _command.User.UserId;
            session.RequestedScopes = _command.RequestedScopes;
        };

        public string CreatedId { set => _command.SessionId = value; }
    }
}
