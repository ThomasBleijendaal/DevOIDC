using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Cms.Core.Models;
using DevOidc.Core.Models.Dtos;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Core.Repositories
{
    public class ClientRepository : BaseMappedRepository<ClientCmsModel, ClientDto>
    {
        private readonly IClientManagementService _clientManagementService;

        public ClientRepository(
            IClientManagementService clientManagementService)
        {
            _clientManagementService = clientManagementService;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return;
            }

            await _clientManagementService.DeleteClientAsync(parent.Entity.Id, id);
        }

        public override async Task<IEnumerable<ClientCmsModel>> GetAllAsync(IParent? parent, IQuery<ClientDto> query)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return Enumerable.Empty<ClientCmsModel>();
            }

            var clients = await _clientManagementService.GetAllClientsAsync(parent.Entity.Id);
            return clients.MapToClientCmsModel();
        }

        public override async Task<ClientCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            if (string.IsNullOrWhiteSpace(parent?.Entity.Id))
            {
                return default;
            }

            var client = await _clientManagementService.GetClientByIdAsync(parent.Entity.Id, id);
            return client?.MapToClientCmsModel();
        }

        public override async Task<ClientCmsModel?> InsertAsync(IEditContext<ClientCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Parent?.Entity.Id))
            {
                return default;
            }

            var clientId = await _clientManagementService.CreateClientAsync(editContext.Parent.Entity.Id, editContext.Entity.MapToClientDto());
            return await GetByIdAsync(clientId, editContext.Parent);
        }

        public override Task<ClientCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new ClientCmsModel { });
        }

        public override async Task UpdateAsync(IEditContext<ClientCmsModel> editContext)
        {
            if (string.IsNullOrWhiteSpace(editContext.Entity.Id) ||
                   string.IsNullOrWhiteSpace(editContext.Parent?.Entity.Id))
            {
                return;
            }

            await _clientManagementService.UpdateClientAsync(editContext.Parent.Entity.Id, editContext.Entity.Id, editContext.Entity.MapToClientDto());
        }
    }
}
