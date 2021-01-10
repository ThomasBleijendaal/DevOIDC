namespace DevOidc.Functions.Models
{
    public class AzureAdConfig
    {
        public string TenantId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ValidAudience { get; set; } = default!;
        public string Instance { get; set; } = default!;
        public string Issuer { get; set; } = default!;
    }
}
