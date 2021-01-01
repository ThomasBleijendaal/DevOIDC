namespace DevOidc.Repositories.Entities
{
    public class SessionEntity : BaseEntity
    {
        public string? UserId { get; set; }
        public string? ClientId { get; set; }
        public string? ScopeId { get; set; }
        public string? RequestedScopes { get; set; }
    }
}
