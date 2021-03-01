namespace DevOidc.Business.Abstractions.Request
{
    public interface IOidcAuthorization
    {
        string Type { get; }
        string Value { get; }
    }
}
