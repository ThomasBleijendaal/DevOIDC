﻿using System;
using System.Linq.Expressions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Specifications
{
    public class GetUserByPasswordSpecification : UserSpecificationBase, ISpecification<UserEntity, UserDto>
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _userName;
        private readonly string _password;

        public GetUserByPasswordSpecification(string tenantId, string clientId, string userName, string password)
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _userName = userName;
            _password = password;
        }

        public Expression<Func<UserEntity, bool>> Criteria => user =>
            user.PartitionKey == _tenantId &&
            user.Clients!.CompareTo(_clientId) >= 0 &&
            user.UserName == _userName &&
            user.Password == _password;
    }
}
