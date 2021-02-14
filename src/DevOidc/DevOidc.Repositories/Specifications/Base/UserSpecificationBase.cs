using System;
using System.Collections.Generic;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Specifications.Base
{
    public class UserSpecificationBase
    {
        public Func<UserEntity, UserDto> Projection => user => new UserDto
        {
            AccessTokenExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(user.AccessTokenExtraClaims ?? "") ?? new Dictionary<string, string>(),
            IdTokenExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(user.IdTokenExtraClaims ?? "") ?? new Dictionary<string, string>(),
            UserInfoExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(user.UserInfoExtraClaims ?? "") ?? new Dictionary<string, string>(),
            FullName = user.FullName ?? "",
            UserId = user.RowKey,
            UserName = user.UserName ?? "",
            Password = user.Password ?? "",
            Clients = JsonConvert.DeserializeObject<List<string>>(user.Clients ?? "") ?? new List<string>()
        };
    }
}
