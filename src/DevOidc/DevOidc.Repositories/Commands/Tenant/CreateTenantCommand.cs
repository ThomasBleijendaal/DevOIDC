using DevOidc.Core.Models.Dtos;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands.Tenant
{
    public class CreateTenantCommand : ICommand
    {
        public CreateTenantCommand(string ownerName, TenantDto tenant, string publicKey, string privateKey)
        {
            OwnerName = ownerName;
            Tenant = tenant;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public string OwnerName { get; }
        public TenantDto Tenant { get; }
        public string PublicKey { get; }
        public string PrivateKey { get; }
        public string? TenantId { get; internal set; }
    }
}
