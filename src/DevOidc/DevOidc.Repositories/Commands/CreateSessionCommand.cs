using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands
{
    public class CreateSessionCommand : ICommand
    {
        public CreateSessionCommand(string tenantId, UserDto user, ClientDto client, ScopeDto scope, string? requestedScopes)
        {
            TenantId = tenantId;
            User = user;
            Client = client;
            Scope = scope;
            RequestedScopes = requestedScopes;
        }

        public string TenantId { get; }
        public UserDto User { get; }
        public ClientDto Client { get; }
        public ScopeDto Scope { get; }
        public string? RequestedScopes { get; }

        public string? SessionId { get; internal set; }
    }
}
