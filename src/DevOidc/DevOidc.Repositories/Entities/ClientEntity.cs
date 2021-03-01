namespace DevOidc.Repositories.Entities
{
    public class ClientEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? ClientSecret { get; set; }
        public string? Scopes { get; set; }
        public string? AccessTokenExtraClaims { get; set; }
        public string? IdTokenExtraClaims { get; set; }
        public string? RedirectUris { get; set; }
    }
}
