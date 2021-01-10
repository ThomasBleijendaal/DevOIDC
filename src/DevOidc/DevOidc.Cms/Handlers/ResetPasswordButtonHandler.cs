
using System.Threading.Tasks;
using DevOidc.Cms.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace DevOidc.Cms.Handlers
{
    public class ResetPasswordButtonHandler : IButtonActionHandler
    {
        public Task ButtonClickAfterRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            if (editContext.Entity is UserCmsModel user)
            {
                user.ResetPassword = true;
            }

            return Task.FromResult(CrudType.Update);
        }

        public OperationAuthorizationRequirement GetOperation(IButton button, FormEditContext editContext)
        {
            return Operations.Update;
        }

        public bool IsCompatible(IButton button, FormEditContext editContext)
        {
            return editContext.EntityState == EntityState.IsExisting && editContext.Entity is UserCmsModel;
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
