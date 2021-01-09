using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Commands.Tenant;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.Client
{
    public class DeleteClientsOfTenantSelection : ISelection<ClientEntity>
    {
        private readonly string _tenantId;
        private readonly string? _clientId;

        public DeleteClientsOfTenantSelection(DeleteTenantCommand command)
        {
            _tenantId = command.TenantId;
        }

        public DeleteClientsOfTenantSelection(DeleteClientCommand command)
        {
            _tenantId = command.TenantId;
            _clientId = command.ClientId;
        }

        public Expression<Func<ClientEntity, bool>> Criteria => _clientId == null 
            ? client => client.PartitionKey == _tenantId
            : client => client.PartitionKey == _tenantId && client.RowKey == _clientId;
    }
}
