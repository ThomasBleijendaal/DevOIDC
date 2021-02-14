using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.User;

namespace DevOidc.Business.Tenant
{
    public class UserService : IUserService
    {
        private readonly IReadRepository<UserEntity> _readRepository;

        public UserService(IReadRepository<UserEntity> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(string tenantId, string userId)
            => await _readRepository.GetAsync(new GetUserByIdSpecification(tenantId, userId));
    }
}
