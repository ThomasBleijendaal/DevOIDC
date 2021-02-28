namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcCodeRequest : IOidcTokenRequest
    {
        string? Code { get; }
    }
}
