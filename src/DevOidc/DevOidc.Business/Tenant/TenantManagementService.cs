using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DevOidc.Business.Tenant
{
    public class TenantManagementService : ITenantManagementService
    {
        private readonly ICommandHandler<CreateTenantCommand> _createTenantCommandHandler;

        public TenantManagementService(ICommandHandler<CreateTenantCommand> createTenantCommandHandler)
        {
            _createTenantCommandHandler = createTenantCommandHandler;
        }

        public async Task CreateTenantAsync(string ownerId)
        {
            var key = CreatePrivateKey();
            
            var command = new CreateTenantCommand(ownerId, GetStringFromKey(key.Public), GetStringFromKey(key.Private));
            await _createTenantCommandHandler.HandleAsync(command);
        }

        private static string GetStringFromKey(AsymmetricKeyParameter param)
        {
            var keyWriter = new StringWriter();
            var pemWriter = new PemWriter(keyWriter);

            pemWriter.WriteObject(param);

            return keyWriter.ToString();
        }

        public static AsymmetricCipherKeyPair CreatePrivateKey()
        {   
            using var rsaProvider = new RSACryptoServiceProvider(2048);
            var rsaKeyInfo = rsaProvider.ExportParameters(true);
            return DotNetUtilities.GetRsaKeyPair(rsaKeyInfo);
        }
    }
}
