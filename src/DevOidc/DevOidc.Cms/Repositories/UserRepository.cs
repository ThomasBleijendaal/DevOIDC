using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOidc.Cms.Models;
using DevOidc.Core.Models;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Repositories
{
    public class UserRepository : BaseMappedRepository<UserCmsModel, UserDto>
    {
        private readonly HttpClient _httpClient;

        public UserRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("cms");
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
            {
                throw new InvalidOperationException();
            }

            await _httpClient.DeleteAsync($"user/{tenant.Id}/{id}");
        }

        public override async Task<IEnumerable<UserCmsModel>> GetAllAsync(IParent? parent, IQuery<UserDto> query)
        {
            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
            {
                throw new InvalidOperationException();
            }

            var users = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await _httpClient.GetStringAsync($"user/{tenant.Id}"));
            if (users == null)
            {
                return Enumerable.Empty<UserCmsModel>();
            }

            return users.Select(user => user.MapToUserCmsModel());
        }

        public override async Task<UserCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            if (parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
            {
                throw new InvalidOperationException();
            }

            var user = JsonConvert.DeserializeObject<UserDto>(await _httpClient.GetStringAsync($"user/{tenant.Id}/{id}"));

            return user?.MapToUserCmsModel();
        }

        public override async Task<UserCmsModel?> InsertAsync(IEditContext<UserCmsModel> editContext)
        {
            if (editContext.Parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
            {
                throw new InvalidOperationException();
            }

            var user = editContext.Entity.MapToUserDto();

            SetClientsFromRelationContainer(editContext, user);

            var response = await _httpClient.PostAsync($"user/{tenant.Id}",
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var userId = await response.Content.ReadAsStringAsync();

            return await GetByIdAsync(userId, editContext.Parent);
        }

        public override Task<UserCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new UserCmsModel());
        }

        public override async Task UpdateAsync(IEditContext<UserCmsModel> editContext)
        {
            if (editContext.Parent?.Entity is not TenantCmsModel tenant || string.IsNullOrWhiteSpace(tenant.Id))
            {
                throw new InvalidOperationException();
            }

            var user = editContext.Entity.MapToUserDto();
            if (editContext.Entity.ResetPassword)
            {
                user.Password = "reset";
            }

            SetClientsFromRelationContainer(editContext, user);

            await _httpClient.PutAsync($"user/{tenant.Id}/{editContext.Entity.Id}",
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
        }

        private static void SetClientsFromRelationContainer(IEditContext<UserCmsModel> editContext, UserDto user)
        {
            user.Clients = editContext.GetRelationContainer().GetRelatedElementIdsFor<ClientCmsModel, string>()?.ToList() ?? user.Clients;
        }
    }
}
