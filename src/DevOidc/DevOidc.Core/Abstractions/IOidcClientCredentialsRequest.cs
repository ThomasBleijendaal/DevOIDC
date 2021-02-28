using System.Collections.Generic;

namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcClientCredentialsRequest : IOidcTokenRequest
    {
        string? ClientId { get; }
        string? ClientSecret { get; }
        string? Scope { get; }
        IEnumerable<string> Scopes { get; }
    }
}
