using System;
using System.Text;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Tenant
{
    public class TenantCreation : ICreation<TenantEntity>
    {
        private readonly CreateTenantCommand _command;

        public TenantCreation(CreateTenantCommand command)
        {
            _command = command;
        }

        public string PartitionKey => _command.OwnerName;

        public Action<TenantEntity> Mutation => tenant =>
        {
            tenant.Name = _command.Tenant.Name;
            tenant.Description = _command.Tenant.Description;
            tenant.TokenLifetime = _command.Tenant.TokenLifetime.ToString();

            tenant.PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(_command.PrivateKey));
            tenant.PublicKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(_command.PublicKey));
        };

        public string CreatedId { set => _command.TenantId = value; }
    }
}
