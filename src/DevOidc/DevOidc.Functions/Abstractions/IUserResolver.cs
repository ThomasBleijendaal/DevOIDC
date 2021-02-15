using System.Security.Claims;

namespace DevOidc.Functions.Abstractions
{
    public interface IUserResolver
    {
        ClaimsPrincipal ResolveUser();
    }
}
