using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope, IEnumerable<string> requestedScopes, string? nonce);
        Task<string> CreateLongLivedSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope, IEnumerable<string> requestedScopes, string? nonce);

        Task<SessionDto?> GetSessionAsync(string tenantId, string code);
        Task<SessionDto?> GetLongLivedSessionAsync(string tenantId, string code);
    }
}
