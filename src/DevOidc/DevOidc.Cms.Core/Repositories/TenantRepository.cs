using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Cms.Core.Abstractions;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Core.Repositories
{
    public class TenantRepository : BaseMappedRepository<TenantCmsModel, TenantDto>
    {
        private readonly ITenantService _tenantService;
        private readonly ITenantManagementService _tenantManagementService;
        private readonly IUserResolver _userResolver;

        public TenantRepository(
            ITenantService tenantService,
            ITenantManagementService tenantManagementService,
            IUserResolver userResolver)
        {
            _tenantService = tenantService;
            _tenantManagementService = tenantManagementService;
            _userResolver = userResolver;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            var user = _userResolver.ResolveUser();
            await _tenantManagementService.DeleteTenantAsync(user.Identity?.Name ?? "-unknown-", id);
        }

        public override async Task<IEnumerable<TenantCmsModel>> GetAllAsync(IParent? parent, IQuery<TenantDto> query)
        {
            var user = _userResolver.ResolveUser();
            var tenants = await (query.ActiveTab == 1
                ? _tenantManagementService.GetTenantsOfOthersAsync(user.Identity?.Name ?? "-unknown-")
                : _tenantManagementService.GetTenantsAsync(user.Identity?.Name ?? "-unknown-"));
            return tenants.Select(x => x.MapToTenantCmsModel());
        }

        public override async Task<TenantCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            var tenant = await _tenantService.GetTenantAsync(id);
            return tenant?.MapToTenantCmsModel();
        }

        public override async Task<TenantCmsModel?> InsertAsync(IEditContext<TenantCmsModel> editContext)
        {
            var user = _userResolver.ResolveUser();
            var tenantId = await _tenantManagementService.CreateTenantAsync(user.Identity?.Name ?? "-unknown-", editContext.Entity.MapToTenantDto());
            return await GetByIdAsync(tenantId, default);
        }

        public override Task<TenantCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new TenantCmsModel { Id = Guid.NewGuid().ToString() });
        }

        public override async Task UpdateAsync(IEditContext<TenantCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Entity.Id))
            {
                return;
            }

            var user = _userResolver.ResolveUser();
            await _tenantManagementService.ClaimTenantAsync(user.Identity?.Name ?? "-unknown-", editContext.Entity.Id);
        }
    }
}
