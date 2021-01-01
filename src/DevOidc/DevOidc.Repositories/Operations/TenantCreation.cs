using System;
using System.Text;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations
{
    public class TenantCreation : ICreation<TenantEntity>
    {
        private readonly CreateTenantCommand _command;

        public TenantCreation(CreateTenantCommand command)
        {
            _command = command;
        }

        public string OwnerId => _command.OwnerId;

        public Action<TenantEntity> Mutation => tenant =>
        {
            tenant.PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(_command.PrivateKey));
            tenant.PublicKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(_command.PublicKey));
        };

        public string CreatedId { private get; set; } = string.Empty;
    }
}
