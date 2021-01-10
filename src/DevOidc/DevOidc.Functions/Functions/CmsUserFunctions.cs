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
    public class CmsUserFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly IUserManagementService _userManagementService;

        public CmsUserFunctions(
            ITenantService tenantService,
            IUserManagementService userManagementService)
        {
            _tenantService = tenantService;
            _userManagementService = userManagementService;
        }

        [FunctionName(nameof(CreateUserAsync))]
        public async Task<IActionResult> CreateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/user/{tenantId}")] HttpRequest req,
            string tenantId)
        {
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
            var user = await _userManagementService.GetUserByIdAsync(tenantId, userId);

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(GetUsersAsync))]
        public async Task<IActionResult> GetUsersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/user/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            var users = await _userManagementService.GetAllUsersAsync(tenantId);

            return new OkObjectResult(users);
        }

        [FunctionName(nameof(UpdateUserAsync))]
        public async Task<IActionResult> UpdateUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "cms/user/{tenantId}/{userId}")] HttpRequest req,
            string tenantId,
            string userId)
        {
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
            // TODO: check owner

            await _userManagementService.DeleteUserAsync(tenantId, userId);

            return new OkObjectResult(tenantId);
        }
    }
}
