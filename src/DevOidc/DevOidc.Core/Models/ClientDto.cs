using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class ClientDto
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<ScopeDto> Scopes { get; set; }
        public Dictionary<string, string> ExtraClaims { get; set; }
    }
}
