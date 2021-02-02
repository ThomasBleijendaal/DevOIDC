using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class UserDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public List<string> Clients { get; set; } = default!;
        public Dictionary<string, string> AccessTokenExtraClaims { get; set; } = default!;
        public Dictionary<string, string> IdTokenExtraClaims { get; set; } = default!;
        public Dictionary<string, string> UserInfoExtraClaims { get; set; } = default!;
    }
}
