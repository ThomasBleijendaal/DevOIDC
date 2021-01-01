using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Commands
{
    public class CreateTenantCommand : ICommand
    {
        public CreateTenantCommand(string ownerId, string publicKey, string privateKey)
        {
            OwnerId = ownerId;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public string OwnerId { get; }
        public string PublicKey { get; }
        public string PrivateKey { get; }
    }
}
