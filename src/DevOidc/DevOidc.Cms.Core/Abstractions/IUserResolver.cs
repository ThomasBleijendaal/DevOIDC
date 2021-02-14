using System.Security.Claims;

namespace DevOidc.Cms.Core.Abstractions
{
    public interface IUserResolver
    {
        ClaimsPrincipal ResolveUser();
    }
}
