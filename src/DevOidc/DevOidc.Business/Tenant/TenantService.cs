using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Providers;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications;

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
            => await _userRepository.GetAsync(new GetUserByPasswordSpecification(tenantId, clientId, userName, password));

        public async Task<UserDto?> GetUserAsync(string tenantId, string clientId, string userId)
            => await _userRepository.GetAsync(new GetUserByIdSpecification(tenantId, userId, clientId));

        public async Task<TenantDto?> GetTenantAsync(string tenantId)
            => await _tenantRepository.GetAsync(new GetTenantSpecification(tenantId));

        public async Task<ClientDto?> GetClientAsync(string tenantId, string clientId)
            => await _clientRepository.GetAsync(new GetClientSpecification(tenantId, clientId));
        
        public async Task<IEncryptionProvider?> GetEncryptionProviderAsync(string tenantId)
        {
            var key = await _tenantRepository.GetAsync(new GetTenantPrivateKeySpecification(tenantId));

            if (key?.PrivateKey is string privateKey &&
                key?.PublicKey is string publicKey)
            {
                return new RS256EncryptionProvider(publicKey, privateKey);
            }

            // return default;

            return new RS256EncryptionProvider(
                publicKey: @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtQ3bNYEPo+el72Pl8Tf6
R1X9l7xHIxykaWqMgYNMg2h0eRQLC6ZCYKUe7vSXjMSDv1tm+F+mfRlsEie47RES
Xw5gn+csPRrMubSEnrJePEYZWwJqCPtvpgR77GtOHN5Kn7FwFJixVuPGr9m6maZ+
Vq32hulhOdnKSGrjLxIjPzPz7kHnTxnUYKvd1radCuSzuJ4fYPuV09hTQU/AT1ul
RLjUaZgeED31jEU7mbMjgCQ4SRG5i/1oJlMuPhWivAWMXIGTB6+82ls5N7sSLwW4
kkkQqqWgPiYociT7ftXCDbC8PODbFFu2YtpteI9h86HP3Px7QfsfFJh03kidgG4Z
aQIDAQAB
-----END PUBLIC KEY-----",
                privateKey: @"-----BEGIN RSA PRIVATE KEY-----
Proc-Type: 4,ENCRYPTED
DEK-Info: AES-256-CBC,957C39F64BC36D415C4D054EA304D427

kbaaKhwyNuHJ8xelXXAe00LxoKvXo61PtW1d6yQStkLZC9AF7UlxvOF9vQ4P25js
v9ZX4zCKfX6Z4v38c2zn8YXhaTBMN+UjZr1rfCp+S6EjEItTp8eFS1Boomnv/htM
OpiU7OpPsmM8nkPQArWEFawZUs/2K/YAdTV4hkycVJjSE11y5kxguo0HtxFjXHKE
wsVsV5p/n3JZBpGDYGF+0F9bYyXT+avRi+JjqfksxJC++rGdpucIVm1YS9KA3BlR
2pH2XsPO8yeBhw0fzpH2/+xZAPTgR36cU/J79MraZ6PrEAI5b6kPmIp4mLP+RXNH
hjrTfe45xoJSmRzlT8mDzjIwrQNg2uAivZapp9cB1xYUO6MOxtF8wpgE9GSuTjxG
h70D5mUeQ6lM+4cPsnICTXUjbWnZwcIHpWX/0q2+RW0HeWxthIvy+agFdi/BIFbi
SzS8D0HWDEv8vgMPaLJuJKm4PPjPanG50bU4iIhuZEez4IBaXADeIutmM3cnGZg0
i77FuS13YEA1lgSebnls8wJqQxENR7fyS9tGq+Paui1a/l/uT4NFYKNqNPhIbqJs
MmCFp16+iH0iKcuwiT91Ahrs72DLl6FF/Ax1DCWUEctFGe7vIGqT56gQFZkgldmg
FF5mfqpO86UTJUiAT+VWKtWIHI4iLemCNpnxt4J3EZhpJgp87kLcS9Ri3dIlBVr7
vsLsg6Zk6wI9RtOFcLLxdVN8s4ywvvFEDoaZOwxxzl4tpT/KgchTAMzgfV9+iHCI
lqE0bOt1NaGFcG3XhbzD3WYTxiVjtcMRrw0J9C2cc+k4+UXHHRaf7r+s4kADcK3u
3YSYoaqsb+q8q56uE1QT1cOxh2plhNbigZFpRVVV5/daCF67nYqVUfQ8NgUf5wXR
3lL1IfPo3CAz949QNx5WrO3KYuuCoKdPydz2OJCkelA9FDykfD9x40ZcQKeZeWd8
0oyGiSyvPLiVrz8IoWK81RRtMUlm955B3O6+Ho4JrPcjRIXlGYq/9JaivDuiOpB9
l+PdmrR3YsPj+TfAtgMbnkUORQeygd8N78Syw1O4XVYUHCAPXawia8hadw0Kiz2Z
l3HbNmQ3Q1WRvsKNCcM9uGM6PC5ntefwYfeXuO+3EjNuA0m3g079WkJiaWTJGPYU
fc1lVI7m5bACPZnAivhCdryaHu3kl5IUVF+q11uEz4qcq+Xwdd/yhSn4JxN8G9EX
hGepXqabrS/tjJrRmgIadI910oTR03d3Pl4Itgr59HYMMjoBrRmF1oz+mScB1KUG
jTgV/O0PfuSmHqg4VJHm5t/stpyGZO9ELochAAPeY4SpmnK9UjFZ1vCd3Ms2Q6GR
Dp2n+wNdLCp2vPJ911cmJeDsuz47pa+EfK49EIJYOhRbgp7CvU0OHk8e7IpnOeW5
uEpECA/t7yW94TIZNdGQvuAVLaG/G+23d16Dy4Kftw9hKVus5bvCFlg+ltPnbAYz
HLU4qpuZMU5UnXWcCDCKeZlwH/kyWnCm2+NQfRGKM1aHOuYEjPBeBtnw9DldFcXK
bga1nySDI5Zvzbxr10rHLo9jRjz3AtU/jzKE9AJGl11qJRWvozy1dmDSsOseEijo
-----END RSA PRIVATE KEY-----"
                );
        }
    }
}
