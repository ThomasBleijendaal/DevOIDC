namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcToken
    {
        string? TokenType { get; }
        string? Scope { get; }
        int ExpiresIn { get; }
        string? AccessToken { get; }
        string? RefreshToken { get; }
        string? IdToken { get; }
    }
}
