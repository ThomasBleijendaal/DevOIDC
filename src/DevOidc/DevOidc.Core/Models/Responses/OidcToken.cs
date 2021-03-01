using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Responses
{
    public class OidcToken : IOidcToken
    {
        public string? TokenType { get; set; }

        public string? Scope { get; set; }

        public int ExpiresIn { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public string? IdToken { get; set; }
    }
}
