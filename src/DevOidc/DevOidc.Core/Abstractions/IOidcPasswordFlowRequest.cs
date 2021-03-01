using System.Collections.Generic;

namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcPasswordFlowRequest : IOidcTokenRequest
    {
        string? UserName { get; }
        string? Password { get; }
        string? ClientId { get; }
        string? Scope { get; }
        IEnumerable<string> Scopes { get; }
        string? Audience { get; }
        string? ResponseType { get; }
    }
}
