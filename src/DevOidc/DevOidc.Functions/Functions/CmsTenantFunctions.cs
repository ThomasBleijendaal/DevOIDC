using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using DevOidc.Functions.Abstractions;
using DevOidc.Functions.Functions.Base;
using DevOidc.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DevOidc.Functions.Functions
{
    public class CmsTenantFunctions : BaseAdAuthenticatedFunctions
    {
        private readonly IAuthenticationValidator _authenticationValidator;
        private readonly ITenantService _tenantService;
        private readonly ITenantManagementService _tenantManagementService;

        public CmsTenantFunctions(
            IOptions<AzureAdConfig> options,
            IAuthenticationValidator authenticationValidator,
            ITenantService tenantService,
            ITenantManagementService tenantManagementService) : base(options, authenticationValidator)
        {
            _authenticationValidator = authenticationValidator;
            _tenantService = tenantService;
            _tenantManagementService = tenantManagementService;
        }

        [FunctionName(nameof(CreateTenantAsync))]
        public async Task<IActionResult> CreateTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/tenant")] HttpRequest req)
        {
            var user = await GetValidUserAsync();

            var body = JsonConvert.DeserializeObject<TenantDto>(await req.ReadAsStringAsync());
            var tenantId = await _tenantManagementService.CreateTenantAsync(user.Identity?.Name ?? "-unknown-", body);

            return new OkObjectResult(tenantId);
        }

        [FunctionName(nameof(DeleteTenantAsync))]
        public async Task<IActionResult> DeleteTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cms/tenant/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            var user = await GetValidUserAsync();
            await _tenantManagementService.DeleteTenantAsync(user.Identity?.Name ?? "-unknown-", tenantId);

            return new OkObjectResult(tenantId);
        }

        [FunctionName(nameof(GetTenantAsync))]
        public async Task<IActionResult> GetTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/tenant/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            await GetValidUserAsync();
            var tenant = await _tenantService.GetTenantAsync(tenantId);

            return new OkObjectResult(tenant);
        }

        [FunctionName(nameof(GetMyTenantsAsync))]
        public async Task<IActionResult> GetMyTenantsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/tenant")] HttpRequest req)
        {
            var user = await GetValidUserAsync();
            var tenants = await _tenantManagementService.GetTenantsAsync(user.Identity?.Name ?? "-unknown-");

            return new OkObjectResult(tenants);
        }

        [FunctionName(nameof(GetOtherTenantsAsync))]
        public async Task<IActionResult> GetOtherTenantsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/tenant/other")] HttpRequest req)
        {
            var user = await GetValidUserAsync();
            var tenants = await _tenantManagementService.GetTenantsOfOthersAsync(user.Identity?.Name ?? "-unknown-");

            return new OkObjectResult(tenants);
        }
    }
}
