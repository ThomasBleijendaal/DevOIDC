using System;
using System.Linq.Expressions;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Entities;

namespace DevOidc.Repositories.Operations.User
{
    public class DeleteClientSelection : ISelection<ClientEntity>
    {
        private readonly string _clientId;
        private readonly string _tenantId;

        public DeleteClientSelection(string clientId, string tenantId)
        {
            _clientId = clientId;
            _tenantId = tenantId;
        }

        public Expression<Func<ClientEntity, bool>> Criteria => client => client.RowKey == _clientId && client.PartitionKey == _tenantId;
    }
}
