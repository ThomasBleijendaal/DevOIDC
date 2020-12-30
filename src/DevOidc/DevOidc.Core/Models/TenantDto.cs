using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class TenantDto
    {
        public string TenantId { get; set; }

        public List<UserDto> Users { get; set; }
    }
}
