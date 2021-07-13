using System.Collections.Generic;
using System.Linq;
using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Requests
{
    public class OidcPasswordFlowRequest : IOidcPasswordFlowRequest
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? ClientId { get; set; }

        public string? Scope { get; set; }

        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

        public string? Audience { get; set; }

        public string? RedirectUri { get; set; }

        public string TenantId { get; init; } = default!;

        public string? ResponseType { get; set; }

        public string? Nonce { get; set; }
    }
}
