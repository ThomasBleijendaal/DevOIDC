﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models.Dtos;

namespace DevOidc.Business.Abstractions
{
    public interface IUserManagementService
    {
        Task<IReadOnlyList<UserDto>> GetAllUsersAsync(string tenantId);
        Task DeleteUserAsync(string tenantId, string userId);
        Task<string> CreateUserAsync(string tenantId, UserDto user);
        Task UpdateUserAsync(string tenantId, string userId, UserDto user, bool resetPassword = false);
    }
}
