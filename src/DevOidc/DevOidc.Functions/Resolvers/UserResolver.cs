using System;
using System.Security.Claims;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Authentication;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Resolvers
{
    public class UserResolver : IUserResolver
    {
        private readonly IFunctionExecutionContextAccessor _functionExecutionContextAccessor;

        public UserResolver(IFunctionExecutionContextAccessor functionExecutionContextAccessor)
        {
            _functionExecutionContextAccessor = functionExecutionContextAccessor;
        }

        public ClaimsPrincipal ResolveUser() 
            => (_functionExecutionContextAccessor.FunctionExecutionContext != null &&
                _functionExecutionContextAccessor.FunctionExecutionContext.Items.TryGetValue(AuthenticationMiddleware.ContextUser, out var user)
                    ? user as ClaimsPrincipal
                    : default)
                ?? throw new UnauthorizedAccessException();
    }
}
