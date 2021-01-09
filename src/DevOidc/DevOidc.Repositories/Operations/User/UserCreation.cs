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
            user.ExtraClaims = JsonConvert.SerializeObject(_command.User.ExtraClaims);
            user.FullName = _command.User.FullName;
            user.UserName = _command.User.UserName;

            user.Password = Guid.NewGuid().ToString().Substring(0, 8);
        };

        public string CreatedId { set => _command.UserId = value; }
    }
}
