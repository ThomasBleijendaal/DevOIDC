using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace DevOidc.Functions.Functions
{
    // TODO: check authentication
    public class CmsTenantFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly ITenantManagementService _tenantManagementService;

        public CmsTenantFunctions(
            ITenantService tenantService,
            ITenantManagementService tenantManagementService)
        {
            _tenantService = tenantService;
            _tenantManagementService = tenantManagementService;
        }

        [FunctionName(nameof(CreateTenantAsync))]
        public async Task<IActionResult> CreateTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/tenant")] HttpRequest req)
        {
            var body = JsonConvert.DeserializeObject<TenantDto>(await req.ReadAsStringAsync());
            var tenantId = await _tenantManagementService.CreateTenantAsync("TODO get name from claims", body);

            return new OkObjectResult(tenantId);
        }

        [FunctionName(nameof(DeleteTenantAsync))]
        public async Task<IActionResult> DeleteTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cms/tenant/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            await _tenantManagementService.DeleteTenantAsync(tenantId);

            return new OkObjectResult(tenantId);
        }

        [FunctionName(nameof(GetTenantAsync))]
        public async Task<IActionResult> GetTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/tenant/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);

            return new OkObjectResult(tenant);
        }

        [FunctionName(nameof(GetTenantsAsync))]
        public async Task<IActionResult> GetTenantsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/tenant")] HttpRequest req)
        {
            var tenants = await _tenantManagementService.GetTenantsAsync();

            return new OkObjectResult(tenants);
        }
    }
}
