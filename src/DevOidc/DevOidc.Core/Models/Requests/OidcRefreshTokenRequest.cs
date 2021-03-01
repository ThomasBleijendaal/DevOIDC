using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Requests
{
    public class OidcRefreshTokenRequest : IOidcRefreshTokenRequest
    {
        public string? RefreshToken { get; set; }

        public string? RedirectUri { get; set; }

        public string TenantId { get; init; } = default!;
    }
}
