using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOidc.Cms.Models;
using DevOidc.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

namespace DevOidc.Cms.Repositories
{
    public class TenantRepository : BaseRepository<TenantCmsModel>
    {
        private readonly HttpClient _httpClient;

        public TenantRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("cms");
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            await _httpClient.DeleteAsync($"tenant/{id}");
        }

        public override async Task<IEnumerable<TenantCmsModel>> GetAllAsync(IParent? parent, IQuery<TenantCmsModel> query)
        {
            var myTenants = query.ActiveDataView?.Label.ToLower() != "others";
            var urlSuffix = myTenants switch { false => "/other", _ => "" };

            var tenants = JsonConvert.DeserializeObject<IEnumerable<TenantDto>>(await _httpClient.GetStringAsync($"tenant{urlSuffix}"));
            if (tenants == null)
            {
                return Enumerable.Empty<TenantCmsModel>();
            }

            return tenants.Select(tenant => tenant.MapToTenantCmsModel());
        }

        public override async Task<TenantCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            var tenant = JsonConvert.DeserializeObject<TenantDto>(await _httpClient.GetStringAsync($"tenant/{id}"));

            return tenant?.MapToTenantCmsModel();
        }

        public override async Task<TenantCmsModel?> InsertAsync(IEditContext<TenantCmsModel> editContext)
        {
            var response = await _httpClient.PostAsync($"tenant", new StringContent(JsonConvert.SerializeObject(editContext.Entity), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var tenantId = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return default;
            }

            return await GetByIdAsync(tenantId, null);
        }

        public override Task<TenantCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new TenantCmsModel());
        }

        public override Task UpdateAsync(IEditContext<TenantCmsModel> editContext)
        {
            throw new InvalidOperationException();
        }

        public async Task ClaimTenantAsync(string id)
        {
            var response = await _httpClient.PostAsync($"tenant/claim/{id}", new StringContent(""));
            response.EnsureSuccessStatusCode();
        }
    }
}
