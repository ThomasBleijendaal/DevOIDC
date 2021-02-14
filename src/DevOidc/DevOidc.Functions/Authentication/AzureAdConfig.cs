using System;

namespace DevOidc.Functions.Authentication
{
    public class AzureAdConfig
    {
        public string TenantId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string Instance { get; set; } = default!;
        public string ValidAudience { get; set; } = default!;
        public Uri ValidIssuer { get; set; } = default!;
        public Uri Authority => new Uri(new Uri(Instance), TenantId);
    }
}
