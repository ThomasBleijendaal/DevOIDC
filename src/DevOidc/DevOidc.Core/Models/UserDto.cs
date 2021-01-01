using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class UserDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public Dictionary<string, string> ExtraClaims { get; set; } = default!;
    }
}
