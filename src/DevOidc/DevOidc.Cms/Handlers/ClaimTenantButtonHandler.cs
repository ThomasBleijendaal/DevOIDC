using System.Threading.Tasks;
using DevOidc.Cms.Models;
using DevOidc.Cms.Repositories;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace DevOidc.Cms.Handlers
{
    public class ClaimTenantButtonHandler : IButtonActionHandler
    {
        private readonly TenantRepository _tenantRepository;

        public ClaimTenantButtonHandler(TenantRepository tenantRepository)
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
                await _tenantRepository.ClaimTenantAsync(tenant.Id);
            }

            return CrudType.Up;
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
