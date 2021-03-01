namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcRefreshTokenRequest : IOidcTokenRequest
    {
        string? RefreshToken { get; }
    }
}
