using System.Collections.Generic;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface IClaimsProvider
    {
        Dictionary<string, object> CreateClaims(UserDto user, ClientDto client, ScopeDto scope);
    }
}
