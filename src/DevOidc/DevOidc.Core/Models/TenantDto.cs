using System;

namespace DevOidc.Core.Models
{
    public class TenantDto
    {
        public string TenantId { get; set; } = default!;

        public string OwnerName { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public TimeSpan TokenLifetime { get; set; }
    }
}
