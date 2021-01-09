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
    public class CmsClientFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly IClientManagementService _clientManagementService;

        public CmsClientFunctions(
            ITenantService tenantService,
            IClientManagementService clientManagementService)
        {
            _tenantService = tenantService;
            _clientManagementService = clientManagementService;
        }

        [FunctionName(nameof(CreateClientAsync))]
        public async Task<IActionResult> CreateClientAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cms/client/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            var body = JsonConvert.DeserializeObject<ClientDto>(await req.ReadAsStringAsync());
            var clientId = await _clientManagementService.CreateClientAsync(tenantId, body);

            return new OkObjectResult(clientId);
        }


        [FunctionName(nameof(GetClientAsync))]
        public async Task<IActionResult> GetClientAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/client/{tenantId}/{clientId}")] HttpRequest req,
            string tenantId,
            string clientId)
        {
            var client = await _clientManagementService.GetClientByIdAsync(tenantId, clientId);

            return new OkObjectResult(client);
        }

        [FunctionName(nameof(GetClientsAsync))]
        public async Task<IActionResult> GetClientsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cms/client/{tenantId}")] HttpRequest req,
            string tenantId)
        {
            var clients = await _clientManagementService.GetAllClientsAsync(tenantId);

            return new OkObjectResult(clients);
        }

        [FunctionName(nameof(UpdateClientAsync))]
        public async Task<IActionResult> UpdateClientAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "cms/client/{tenantId}/{clientId}")] HttpRequest req,
            string tenantId,
            string clientId)
        {
            var body = JsonConvert.DeserializeObject<ClientDto>(await req.ReadAsStringAsync());
            await _clientManagementService.UpdateClientAsync(tenantId, clientId, body);

            return new OkResult();
        }

        [FunctionName(nameof(DeleteClientAsync))]
        public async Task<IActionResult> DeleteClientAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cms/client/{tenantId}/{clientId}")] HttpRequest req,
            string tenantId,
            string clientId)
        {
            // TODO: check owner

            await _clientManagementService.DeleteClientAsync(tenantId, clientId);

            return new OkObjectResult(tenantId);
        }
    }
}
