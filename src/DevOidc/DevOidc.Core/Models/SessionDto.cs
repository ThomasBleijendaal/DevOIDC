﻿namespace DevOidc.Core.Models
{
    public class SessionDto
    {
        public string? RequestedScopes { get; set; }
        public UserDto User { get; set; }
        public TenantDto Tenant { get; set; }
        public ClientDto Client { get; set; }
        public ScopeDto Scope { get; set; }
    }
}
