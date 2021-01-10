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
    public class CmsUserFunctions : BaseAdAuthenticatedFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly IUserManagementService _userManagementService;

        public CmsUserFunctions(
            IOptions<AzureAdConfig> options,
            IAuthenticationValidator authenticationValidator,
            ITenantService tenantService,
            IUserManagementService userManagementService) : base(options, authenticationValidator)
        {
            _tenantService = tenantService;
            _userManagementService = userManagementService;
        }

        [FunctionName(nameof(CreateUserAsync))]
        public async Task<IActionResult> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/user/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            await GetValidUserAsync();

            var body = JsonConvert.DeserializeObject<UserDto>(await req.ReadAsStringAsync());
            var userId = await _userManagementService.CreateUserAsync(tenantId, body);

            return new OkObjectResult(userId);
        }


        [FunctionName(nameof(GetUserAsync))]
        public async Task<IActionResult> GetUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/user/{tenantId}/{userId}")] HttpRequest req,
            string tenantId,
            string userId)
        {
            await GetValidUserAsync();
            var user = await _userManagementService.GetUserByIdAsync(tenantId, userId);

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(GetUsersAsync))]
        public async Task<IActionResult> GetUsersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/user/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            await GetValidUserAsync();
            var users = await _userManagementService.GetAllUsersAsync(tenantId);

            return new OkObjectResult(users);
        }

        [FunctionName(nameof(UpdateUserAsync))]
        public async Task<IActionResult> UpdateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "cms/user/{tenantId}/{userId}")] HttpRequest req,
            string tenantId,
            string userId)
        {
            await GetValidUserAsync();
            var body = JsonConvert.DeserializeObject<UserDto>(await req.ReadAsStringAsync());
            await _userManagementService.UpdateUserAsync(tenantId, userId, body, body.Password == "reset");

            return new OkResult();
        }

        [FunctionName(nameof(DeleteUserAsync))]
        public async Task<IActionResult> DeleteUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cms/user/{tenantId}/{userId}")] HttpRequest req,
            string tenantId,
            string userId)
        {
            await GetValidUserAsync();
            await _userManagementService.DeleteUserAsync(tenantId, userId);

            return new OkObjectResult(tenantId);
        }
    }
}
