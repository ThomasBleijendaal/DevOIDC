using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope);
        Task<string> CreateLongLivedSessionAsync(string tenantId, UserDto user, ClientDto client, ScopeDto scope);

        Task<SessionDto?> GetSessionAsync(string tenantId, string code);
        Task<SessionDto?> GetLongLivedSessionAsync(string tenantId, string code);
    }
}
