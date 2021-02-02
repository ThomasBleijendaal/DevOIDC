using System.Threading.Tasks;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string tenantId, string userId);
    }
}
