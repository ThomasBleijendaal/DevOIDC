using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Commands.Client;
using DevOidc.Repositories.Entities;
using DevOidc.Repositories.Specifications.Client;

namespace DevOidc.Business.Tenant
{
    public class ClientManagementService : IClientManagementService
    {
        private readonly IReadRepository<ClientEntity> _readRepository;
        private readonly ICommandHandler<CreateClientCommand> _createClientCommandHandler;
        private readonly ICommandHandler<UpdateClientCommand> _updateClientCommandHandler;
        private readonly ICommandHandler<DeleteClientCommand> _deleteClientCommandHandler;

        public ClientManagementService(
            IReadRepository<ClientEntity> readRepository,
            ICommandHandler<CreateClientCommand> createClientCommandHandler,
            ICommandHandler<UpdateClientCommand> updateClientCommandHandler,
            ICommandHandler<DeleteClientCommand> deleteClientCommandHandler)
        {
            _readRepository = readRepository;
            _createClientCommandHandler = createClientCommandHandler;
            _updateClientCommandHandler = updateClientCommandHandler;
            _deleteClientCommandHandler = deleteClientCommandHandler;
        }

        public async Task<string> CreateClientAsync(string tenantId, ClientDto client)
        {
            var command = new CreateClientCommand(tenantId, client);

            await _createClientCommandHandler.HandleAsync(command);

            return command.ClientId ?? throw new InvalidOperationException();
        }

        public async Task DeleteClientAsync(string tenantId, string clientId) 
            => await _deleteClientCommandHandler.HandleAsync(new DeleteClientCommand(tenantId, clientId));

        public async Task<IReadOnlyList<ClientDto>> GetAllClientsAsync(string tenantId) 
            => await _readRepository.GetListAsync(new GetClientsByTenantIdSpecification(tenantId));

        public async Task<ClientDto?> GetClientByIdAsync(string tenantId, string clientId) 
            => await _readRepository.GetAsync(new GetClientByIdSpecification(tenantId, clientId));

        public async Task UpdateClientAsync(string tenantId, string clientId, ClientDto client) 
            => await _updateClientCommandHandler.HandleAsync(new UpdateClientCommand(tenantId, clientId, client));
    }
}
