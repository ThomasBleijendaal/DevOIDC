global using Claim = (string key, string value);

namespace DevOidc2.Domain;

public record Error(string Message);

public static class Result
{
    public static Result<T> Success<T>(T data) => new(data, null);
    public static Result<T> Error<T>(Error error) where T : class => new(null, error);
    public static Result<T> Null<T>() where T : class => new(null, null);
}

public record Result<T>(T? Data, Error? Error)
{
    public bool IsOk => Error == null;
    public bool IsFound => IsOk && Data != null;

    public (T? data, Error? error) Deconstruct()
    {
        return (Data, Error);
    }
}

public record Metadata(
    string[] SupportedClaims);

public record Tenant(
    string Name,
    string Description,
    TimeSpan TokenLifetime);

public record Client(
    string ClientId,
    string ClientSecret,
    string TenantId);

public record User(
    string Username,
    string Password,
    string FullName,
    string[] ClientIds,
    Claim[] AccessTokenExtraClaims,
    Claim[] IdTokenExtraClaims,
    Claim[] UserInfoExtraClaim);

public record Session(
    string Nonce,
    string[] RequestedScopes,
    string UserId,
    string TenantId,
    string ClientId,
    string ScopeId,
    string Audience);

public record KeyMetadata(
    string KeyType,
    string Algorithm,
    string Use,
    string Id,
    string Modulus,
    string Exponent);
