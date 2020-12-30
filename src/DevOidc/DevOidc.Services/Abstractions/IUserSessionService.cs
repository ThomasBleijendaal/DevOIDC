using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface IUserSessionService
    {
        Task<string> StoreClaimsAsync(UserDto user, ClientDto client, ScopeDto scope);
        Task<string> StoreClaimsAsync(Dictionary<string, object> claims);
        Task<Dictionary<string, object>?> GetClaimsByCodeAsync(string code);
    }

}
