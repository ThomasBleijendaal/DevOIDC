using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using DevOidc.Functions.Abstractions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Core.Repositories
{
    public class UserRepository : BaseMappedRepository<UserCmsModel, UserDto>
    {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IUserManagementService _userManagementService;
        private readonly IUserResolver _userResolver;

        public UserRepository(
            ITenantService tenantService,
            IUserService userService,
            IUserManagementService userManagementService,
            IUserResolver userResolver)
        {
            _tenantService = tenantService;
            _userService = userService;
            _userManagementService = userManagementService;
            _userResolver = userResolver;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return;
            }

            await _userManagementService.DeleteUserAsync(parent.Entity.Id, id);
        }

        public override async Task<IEnumerable<UserCmsModel>> GetAllAsync(IParent? parent, IQuery<UserDto> query)
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

            var userId = await _userManagementService.CreateUserAsync(editContext.Parent.Entity.Id, editContext.Entity.MapToUserDto());
            return await GetByIdAsync(userId, editContext.Parent);
        }

        public override Task<UserCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new UserCmsModel { });
        }

        public override async Task UpdateAsync(IEditContext<UserCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Entity.Id) ||
                string.IsNullOrWhiteSpace(editContext.Parent?.Entity.Id))
            {
                return;
            }

            await _userManagementService.UpdateUserAsync(editContext.Parent.Entity.Id, editContext.Entity.Id, editContext.Entity.MapToUserDto(), editContext.Entity.Password == "reset");
        }
    }
}
