using System;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Operations.User
{
    public class UserCreation : ICreation<UserEntity>
    {
        private readonly CreateUserCommand _command;

        public UserCreation(CreateUserCommand command)
        {
            _command = command;
        }

        public string PartitionKey => _command.TenantId;

        public Action<UserEntity> Mutation => user =>
        {
            user.Clients = JsonConvert.SerializeObject(_command.User.Clients);
            user.AccessTokenExtraClaims = JsonConvert.SerializeObject(_command.User.AccessTokenExtraClaims);
            user.IdTokenExtraClaims = JsonConvert.SerializeObject(_command.User.IdTokenExtraClaims);
            user.UserInfoExtraClaims = JsonConvert.SerializeObject(_command.User.UserInfoExtraClaims);
            user.FullName = _command.User.FullName;
            user.UserName = _command.User.UserName;

            user.Password = Guid.NewGuid().ToString().Substring(0, 8);
        };

        public string CreatedId { set => _command.UserId = value; }
    }
}
