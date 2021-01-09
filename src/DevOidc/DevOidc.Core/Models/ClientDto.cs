using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class ClientDto
    {
        public string ClientId { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> RedirectUris { get; set; } = default!;
        public List<ScopeDto> Scopes { get; set; } = default!;
        public Dictionary<string, string> ExtraClaims { get; set; } = default!;
    }
}
