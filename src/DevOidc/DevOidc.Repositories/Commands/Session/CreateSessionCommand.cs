using System.Collections.Generic;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Session
{
    public class CreateSessionCommand : ICommand
    {
        public CreateSessionCommand(string tenantId, UserDto user, ClientDto client, string scopeId, IEnumerable<string> requestedScopes, string? audience)
        {
            TenantId = tenantId;
            User = user;
            Client = client;
            ScopeId = scopeId;
            RequestedScopes = requestedScopes;
            Audience = audience;
        }

        public string TenantId { get; }
        public UserDto User { get; }
        public ClientDto Client { get; }
        public string ScopeId { get; }
        public IEnumerable<string> RequestedScopes { get; }
        public string? Audience { get; }

        public string? SessionId { get; internal set; }
    }
}
