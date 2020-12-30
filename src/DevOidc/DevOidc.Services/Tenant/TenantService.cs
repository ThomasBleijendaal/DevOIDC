using System.Collections.Generic;
using System.Threading.Tasks;
using DevOidc.Core.Models;
using DevOidc.Services.Abstractions;

namespace DevOidc.Services.Tenant
{
    public class TenantService : ITenantService
    {
        public async Task<UserDto?> AuthenticateUserAsync(string tenantId, string clientId, string userName, string password)
        {
            await Task.Delay(100);

            if (userName == "test" && password == "test1234")
            {
                return new UserDto
                {
                    UserId = "12345",
                    UserName = "test@test.com"
                };
            }

            return default;
        }

        public async Task<ClientDto?> GetClientAsync(string tenantId, string clientId)
        {
            await Task.Delay(100);

            return new ClientDto
            {
                Scopes = new List<ScopeDto>
                {
                    new ScopeDto
                    {
                        ScopeId = "abcd",
                        Description = "Default"
                    }
                },
                ClientId = clientId,
                ExtraClaims = new Dictionary<string, string>
                {
                    { "country", "NL" }
                },
                RedirectUris = new List<string>
                {
                    "https://localhost:5001/authentication/login-callback"
                },
                TenantId = tenantId
            };
        }
    }
}
