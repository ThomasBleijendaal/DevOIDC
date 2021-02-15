using System.Collections.Generic;
using DevOidc.Core.Models.Dtos;

namespace DevOidc.Business.Abstractions
{
    public interface IClaimsProvider
    {
        Dictionary<string, object> CreateAccessTokenClaims(UserDto user, ClientDto client, string? audience);
        Dictionary<string, object> CreateIdTokenClaims(UserDto user, ClientDto client, string scope, string? nonce);
        Dictionary<string, object> CreateUserInfoClaims(UserDto user);
    }
}
