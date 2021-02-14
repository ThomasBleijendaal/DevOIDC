//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using DevOidc.Cms.Models;
//using DevOidc.Core.Models.Dtos;
//using Newtonsoft.Json;
//using RapidCMS.Core.Abstractions.Data;
//using RapidCMS.Core.Abstractions.Forms;
//using RapidCMS.Core.Repositories;

//namespace DevOidc.Cms.Repositories
//{
//    public class ClientRepository : BaseRepository<ClientCmsModel>
//    {
//        private readonly HttpClient _httpClient;
//        private readonly UserRepository _userRepository;

//        public ClientRepository(
//            IHttpClientFactory httpClientFactory,
//            UserRepository userRepository)
//        {
//            _httpClient = httpClientFactory.CreateClient("cms");
//            _userRepository = userRepository;
//        }

//        public override async Task DeleteAsync(string id, IParent? parent)
//        {
//            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
//            {
//                throw new InvalidOperationException();
//            }

//            await _httpClient.DeleteAsync($"client/{tenant.Id}/{id}");
//        }

//        public override async Task<IEnumerable<ClientCmsModel>> GetAllAsync(IParent? parent, IQuery<ClientCmsModel> query)
//        {
//            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
//            {
//                throw new InvalidOperationException();
//            }

//            var clients = JsonConvert.DeserializeObject<IEnumerable<ClientDto>>(await _httpClient.GetStringAsync($"client/{tenant.Id}"));
//            if (clients == null)
//            {
//                return Enumerable.Empty<ClientCmsModel>();
//            }

//            return query.ApplyOrder(clients.Select(client => client.MapToClientCmsModel()).AsQueryable());
//        }

//        public override async Task<ClientCmsModel?> GetByIdAsync(string id, IParent? parent)
//        {
//            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
//            {
//                throw new InvalidOperationException();
//            }

//            var client = JsonConvert.DeserializeObject<ClientDto>(await _httpClient.GetStringAsync($"client/{tenant.Id}/{id}"));

//            return client?.MapToClientCmsModel();
//        }

//        public override async Task<ClientCmsModel?> InsertAsync(IEditContext<ClientCmsModel> editContext)
//        {
//            if (editContext.Parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
//            {
//                throw new InvalidOperationException();
//            }

//            var response = await _httpClient.PostAsync($"client/{tenant.Id}",
//                new StringContent(JsonConvert.SerializeObject(editContext.Entity.MapToClientDto()), Encoding.UTF8, "application/json"));

//            response.EnsureSuccessStatusCode();

//            var clientId = await response.Content.ReadAsStringAsync();

//            return await GetByIdAsync(clientId, editContext.Parent);
//        }

//        public override Task<ClientCmsModel> NewAsync(IParent? parent, Type? variantType = null)
//        {
//            return Task.FromResult(new ClientCmsModel());
//        }

//        public override async Task UpdateAsync(IEditContext<ClientCmsModel> editContext)
//        {
//            if (editContext.Parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
//            {
//                throw new InvalidOperationException();
//            }

//            await _httpClient.PutAsync($"client/{tenant.Id}/{editContext.Entity.Id}", 
//                new StringContent(JsonConvert.SerializeObject(editContext.Entity.MapToClientDto()), Encoding.UTF8, "application/json"));
//        }
//    }
//}
