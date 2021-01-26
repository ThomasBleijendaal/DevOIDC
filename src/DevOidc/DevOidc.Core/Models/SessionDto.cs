using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class SessionDto
    {
        public List<string> RequestedScopes { get; set; } = default!;
        public string? Nonce { get; set; }
        public UserDto User { get; set; } = default!;
        public TenantDto Tenant { get; set; } = default!;
        public ClientDto Client { get; set; } = default!;
        public ScopeDto Scope { get; set; } = default!;
    }
}
