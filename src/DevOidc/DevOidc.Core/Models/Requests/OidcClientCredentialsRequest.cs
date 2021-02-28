using System.Collections.Generic;
using System.Linq;
using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Requests
{
    public class OidcClientCredentialsRequest : IOidcClientCredentialsRequest
    {
        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }

        public string? Scope { get; set; }

        public IEnumerable<string> Scopes => Scope?.Split(' ') ?? Enumerable.Empty<string>();

        public string? RedirectUri { get; set; }

        public string TenantId { get; init; } = default!;
    }
}
