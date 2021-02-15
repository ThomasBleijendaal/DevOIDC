using System.Threading.Tasks;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Repositories.ApiBridge;

namespace DevOidc.Cms.Handlers
{
    public class ClaimTenantButtonHandler : IButtonActionHandler
    {
        private readonly ApiMappedRepository<TenantCmsModel, TenantDto> _tenantRepository;

        public ClaimTenantButtonHandler(ApiMappedRepository<TenantCmsModel, TenantDto> tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public Task ButtonClickAfterRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public async Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            if (editContext.Entity is TenantCmsModel tenant && !string.IsNullOrEmpty(tenant.Id))
            {
                await _tenantRepository.UpdateAsync(new FormEditContextWrapper<TenantCmsModel>(editContext));
            }

            return CrudType.Refresh;
        }

        public OperationAuthorizationRequirement GetOperation(IButton button, FormEditContext editContext)
        {
            return Operations.Update;
        }

        public bool IsCompatible(IButton button, FormEditContext editContext)
        {
            return editContext.EntityState == EntityState.IsExisting && editContext.Entity is TenantCmsModel;
        }

        public bool RequiresValidForm(IButton button, FormEditContext editContext)
        {
            return true;
        }

        public bool ShouldAskForConfirmation(IButton button, FormEditContext editContext)
        {
            return true;
        }
    }
}
