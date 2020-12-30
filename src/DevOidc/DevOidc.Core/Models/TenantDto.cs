using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class TenantDto
    {
        public string TenantId { get; set; }

        public List<UserDto> Users { get; set; }
    }

    public class ClientDto
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<ScopeDto> Scopes { get; set; }
        public Dictionary<string, string> ExtraClaims { get; set; }
    }

    public class ScopeDto
    {
        public string ScopeId { get; set; }
        public string Description { get; set; }
    }

    public class UserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
