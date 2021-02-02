using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.User;
using DevOidc.Repositories.Entities;
using Newtonsoft.Json;

namespace DevOidc.Repositories.Operations.User
{
    public class UpdateUserOperation : IOperation<UserEntity>
    {
        private readonly UpdateUserCommand _command;

        public UpdateUserOperation(UpdateUserCommand command)
        {
            _command = command;
        }

        public Action<UserEntity> Mutation => user =>
        {
            user.Clients = JsonConvert.SerializeObject(_command.User.Clients);
            user.AccessTokenExtraClaims = JsonConvert.SerializeObject(_command.User.AccessTokenExtraClaims);
            user.IdTokenExtraClaims = JsonConvert.SerializeObject(_command.User.IdTokenExtraClaims);
            user.UserInfoExtraClaims = JsonConvert.SerializeObject(_command.User.UserInfoExtraClaims);
            user.FullName = _command.User.FullName;
            user.UserName = _command.User.UserName;

            if (_command.UpdatePassword)
            {
                user.Password = Guid.NewGuid().ToString().Substring(0, 8);
            }
        };

        public Expression<Func<UserEntity, bool>> Criteria => user =>
            user.PartitionKey == _command.TenantId &&
            user.RowKey == _command.UserId;
    }
}
