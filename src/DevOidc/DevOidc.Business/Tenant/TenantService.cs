using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Providers;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Client;
using DevOidc.Repositories.Specifications.Tenant;
using DevOidc.Repositories.Specifications.User;

namespace DevOidc.Business.Tenant
{
    public class TenantService : ITenantService
    {
        private readonly IReadRepository<TenantEntity> _tenantRepository;
        private readonly IReadRepository<UserEntity> _userRepository;
        private readonly IReadRepository<ClientEntity> _clientRepository;

        public TenantService(
            IReadRepository<TenantEntity> tenantRepository,
            IReadRepository<UserEntity> userRepository,
            IReadRepository<ClientEntity> clientRepository)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
        }

        public async Task<UserDto?> AuthenticateUserAsync(string tenantId, string clientId, string userName, string password)
        {
            var user = await _userRepository.GetAsync(new GetUserByPasswordSpecification(tenantId, userName, password));

            return CheckIfUserHasAccessToClient(clientId, user);
        }

        public async Task<UserDto?> GetUserAsync(string tenantId, string clientId, string userId)
        {
            var user = await _userRepository.GetAsync(new GetUserByIdSpecification(tenantId, userId));

            return CheckIfUserHasAccessToClient(clientId, user);
        }

        

        public async Task<TenantDto?> GetTenantAsync(string tenantId)
            => await _tenantRepository.GetAsync(new GetTenantSpecification(tenantId));

        public async Task<ClientDto?> GetClientAsync(string tenantId, string clientId)
            => await _clientRepository.GetAsync(new GetClientByIdSpecification(tenantId, clientId));
        
        public async Task<IEncryptionProvider?> GetEncryptionProviderAsync(string tenantId)
        {
            var key = await _tenantRepository.GetAsync(new GetTenantPrivateKeySpecification(tenantId));

            if (key?.PrivateKey is string privateKey &&
                key?.PublicKey is string publicKey)
            {
                return new RS256EncryptionProvider(publicKey, privateKey);
            }

            return default;
        }

        private static UserDto? CheckIfUserHasAccessToClient(string clientId, UserDto? user)
        {
            if (user?.Clients.Contains(clientId) == true)
            {
                return user;
            }

            return default;
        }
    }
}
