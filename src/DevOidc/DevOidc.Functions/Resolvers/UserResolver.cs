using System;
using System.Security.Claims;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Authentication;
using RapidCMS.Api.Functions.Abstractions;

namespace DevOidc.Functions.Resolvers
{
    public class UserResolver : IUserResolver
    {
        private readonly IFunctionContextAccessor _functionContextAccessor;

        public UserResolver(IFunctionContextAccessor functionContextAccessor)
        {
            _functionContextAccessor = functionContextAccessor;
        }

        public ClaimsPrincipal ResolveUser() 
            => (_functionContextAccessor.FunctionExecutionContext != null &&
                _functionContextAccessor.FunctionExecutionContext.Items.TryGetValue(AuthenticationMiddleware.ContextUser, out var user)
                    ? user as ClaimsPrincipal
                    : default)
                ?? throw new UnauthorizedAccessException();
    }
}
