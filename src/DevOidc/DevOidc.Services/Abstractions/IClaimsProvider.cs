using System.Collections.Generic;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface IClaimsProvider
    {
        Dictionary<string, object> CreateAccessTokenClaims(UserDto user, ClientDto client, ScopeDto scope);
        Dictionary<string, object> CreateIdTokenClaims(UserDto user, ClientDto client, ScopeDto scope);
    }
}
