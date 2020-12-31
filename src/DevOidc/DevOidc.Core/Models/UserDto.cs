using System.Collections.Generic;

namespace DevOidc.Core.Models
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Dictionary<string, string> ExtraClaims { get; set; }
    }
}
