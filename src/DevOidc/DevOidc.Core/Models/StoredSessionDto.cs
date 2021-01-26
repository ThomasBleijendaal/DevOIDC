using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class StoredSessionDto
    {
        public List<string> RequestedScopes { get; set; } = default!;
        public string? Nonce { get; set; }
        public string TenantId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ScopeId { get; set; } = default!;
    }
}
