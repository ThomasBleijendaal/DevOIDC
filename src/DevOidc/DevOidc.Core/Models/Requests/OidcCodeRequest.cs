using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Requests
{
    public class OidcCodeRequest : IOidcCodeRequest
    {
        public string? Code { get; set; }

        public string? RedirectUri { get; set; }

        public string TenantId { get; init; } = default!;
    }
}
