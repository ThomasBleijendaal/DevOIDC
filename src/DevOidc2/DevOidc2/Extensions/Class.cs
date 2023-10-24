namespace DevOidc2.Extensions;

public static class HttpContextExtensions
{
    public static string GetBaseUri(this HttpContext context, Guid tenantId)
        => $"{context.Request.Scheme}://{context.Request.Host}/{tenantId}";
}
