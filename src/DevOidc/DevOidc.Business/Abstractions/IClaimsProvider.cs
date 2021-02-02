using System.Collections.Generic;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface IClaimsProvider
    {
        Dictionary<string, object> CreateAccessTokenClaims(UserDto user, ClientDto client, ScopeDto scope);
        Dictionary<string, object> CreateIdTokenClaims(UserDto user, ClientDto client, ScopeDto scope, string? nonce);
        Dictionary<string, object> CreateUserInfoClaims(UserDto user);
    }
}
