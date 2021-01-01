namespace DevOidc.Repositories.Entities
{
    public class ClientEntity : BaseEntity
    {
        public string? Scopes { get; set; }
        public string? ExtraClaims { get; set; }
        public string? RedirectUris { get; set; }
    }
}
