namespace DevOidc.Repositories.Entities
{
    public class TenantEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TokenLifetime { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
    }
}
