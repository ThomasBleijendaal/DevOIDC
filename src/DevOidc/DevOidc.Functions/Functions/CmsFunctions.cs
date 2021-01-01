using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class CmsFunctions
    {
        private readonly ITenantManagementService _tenantManagementService;

        public CmsFunctions(ITenantManagementService tenantManagementService)
        {
            _tenantManagementService = tenantManagementService;
        }

        [FunctionName(nameof(CreateTenantAsync))]
        public async Task<IActionResult> CreateTenantAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/tenant")] HttpRequest req)
        {
            await _tenantManagementService.CreateTenantAsync("");

            return new OkResult();
        }
    }
}
