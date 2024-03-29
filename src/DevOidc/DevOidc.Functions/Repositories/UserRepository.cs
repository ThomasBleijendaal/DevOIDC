﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Cms.Core.Models;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Core.Repositories
{
    public class UserRepository : BaseMappedRepository<UserCmsModel, UserDto>
    {
        private readonly IUserService _userService;
        private readonly IUserManagementService _userManagementService;
        private readonly IClientManagementService _clientManagementService;

        public UserRepository(
            IUserService userService,
            IUserManagementService userManagementService,
            IClientManagementService clientManagementService)
        {
            _userService = userService;
            _userManagementService = userManagementService;
            _clientManagementService = clientManagementService;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return;
            }

            await _userManagementService.DeleteUserAsync(parent.Entity.Id, id);
        }

        public override async Task<IEnumerable<UserCmsModel>> GetAllAsync(IParent? parent, IView<UserDto> query)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return Enumerable.Empty<UserCmsModel>();
            }

            var users = await _userManagementService.GetAllUsersAsync(parent.Entity.Id);
            return users.MapToUserCmsModel();
        }

        public override async Task<UserCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return default;
            }

            var user = await _userService.GetUserByIdAsync(parent.Entity.Id, id);
            return user?.MapToUserCmsModel();
        }

        public override async Task<UserCmsModel?> InsertAsync(IEditContext<UserCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Parent?.Entity.Id))
            {
                return default;
            }

            var user = editContext.Entity.MapToUserDto();

            var clientIds = editContext.GetRelationContainer().GetRelatedElementIdsFor<ClientCmsModel, string>()?.ToList();
            if (clientIds != null)
            {
                user.Clients = clientIds;
            }

            var userId = await _userManagementService.CreateUserAsync(editContext.Parent.Entity.Id, user);
            return await GetByIdAsync(userId, editContext.Parent);
        }

        public override async Task<UserCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return new UserCmsModel();
            }

            var clients = await _clientManagementService.GetAllClientsAsync(parent.Entity.Id);

            return new UserCmsModel
            {
                Id = Guid.NewGuid().ToString(),
                Clients = clients.ToList(x => x.ClientId)
            };
        }

        public override async Task UpdateAsync(IEditContext<UserCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Entity.Id) ||
                string.IsNullOrWhiteSpace(editContext.Parent?.Entity.Id))
            {
                return;
            }

            var user = editContext.Entity.MapToUserDto();

            var clientIds = editContext.GetRelationContainer().GetRelatedElementIdsFor<ClientCmsModel, string>()?.ToList();
            if (clientIds != null)
            {
                user.Clients = clientIds;
            }

            await _userManagementService.UpdateUserAsync(editContext.Parent.Entity.Id, editContext.Entity.Id, user, editContext.Entity.ResetPassword);
        }
    }
}
