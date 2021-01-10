using System;
using System.Collections.Generic;
using DevOidc.Core.Models;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Specifications.Base
{
    public class UserSpecificationBase
    {
        public Func<UserEntity, UserDto> Projection => user => new UserDto
        {
            ExtraClaims = JsonConvert.DeserializeObject<Dictionary<string, string>>(user.ExtraClaims ?? "") ?? new Dictionary<string, string>(),
            FullName = user.FullName ?? "",
            UserId = user.RowKey,
            UserName = user.UserName ?? "",
            Password = user.Password ?? "",
            Clients = JsonConvert.DeserializeObject<List<string>>(user.Clients ?? "") ?? new List<string>()
        };
    }
}
