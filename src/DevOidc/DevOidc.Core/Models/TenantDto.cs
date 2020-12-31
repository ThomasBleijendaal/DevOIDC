using System;
using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class TenantDto
    {
        public string TenantId { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}
