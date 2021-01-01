using System;

namespace DevOidc.Core.Models
{
    public class TenantDto
    {
        public string TenantId { get; set; } = default!;

        public TimeSpan TokenLifetime { get; set; }
    }
}
