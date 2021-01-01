﻿namespace DevOidc.Repositories.Entities
{
    public class UserEntity : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Clients { get; set; }
        public string? ExtraClaims { get; set; }
    }
}
