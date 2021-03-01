using System.Collections.Generic;

namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcTokenRequest
    {
        string? RedirectUri { get; }
        string TenantId { get; }
    }
}
