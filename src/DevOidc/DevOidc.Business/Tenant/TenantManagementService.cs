using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Tenant;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DevOidc.Business.Tenant
{
    public class TenantManagementService : ITenantManagementService
    {
        private readonly IReadRepository<TenantEntity> _readRepository;
        private readonly ICommandHandler<CreateTenantCommand> _createTenantCommandHandler;
        private readonly ICommandHandler<DeleteTenantCommand> _deleteTenantCommandHandler;

        public TenantManagementService(
            IReadRepository<TenantEntity> readRepository,
            ICommandHandler<CreateTenantCommand> createTenantCommandHandler,
            ICommandHandler<DeleteTenantCommand> deleteTenantCommandHandler)
        {
            _readRepository = readRepository;
            _createTenantCommandHandler = createTenantCommandHandler;
            _deleteTenantCommandHandler = deleteTenantCommandHandler;
        }

        public async Task<string> CreateTenantAsync(string ownerName, TenantDto tenant)
        {
            var key = CreatePrivateKey();
            
            var command = new CreateTenantCommand(ownerName, tenant, GetStringFromKey(key.Public), GetStringFromKey(key.Private));
            await _createTenantCommandHandler.HandleAsync(command);

            return command.TenantId ?? throw new InvalidOperationException();
        }

        public async Task<IReadOnlyList<TenantDto>> GetTenantsAsync(string ownerName)
            => await _readRepository.GetListAsync(new GetAllTenantsSpecification(ownerName));

        public async Task<IReadOnlyList<TenantDto>> GetTenantsOfOthersAsync(string ownerName)
            => await _readRepository.GetListAsync(new GetAllOtherTenantsSpecification(ownerName));

        public async Task DeleteTenantAsync(string ownerName, string tenantId)
        {
            await _deleteTenantCommandHandler.HandleAsync(new DeleteTenantCommand(ownerName, tenantId));
        }

        private static string GetStringFromKey(AsymmetricKeyParameter param)
        {
            var keyWriter = new StringWriter();
            var pemWriter = new PemWriter(keyWriter);

            pemWriter.WriteObject(param);

            return keyWriter.ToString();
        }

        private static AsymmetricCipherKeyPair CreatePrivateKey()
        {   
            using var rsaProvider = new RSACryptoServiceProvider(2048);
            var rsaKeyInfo = rsaProvider.ExportParameters(true);
            return DotNetUtilities.GetRsaKeyPair(rsaKeyInfo);
        }
    }
}
