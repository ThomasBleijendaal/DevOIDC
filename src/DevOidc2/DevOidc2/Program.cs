global using System.Text.Json.Serialization;
global using DevOidc2.Extensions;
global using DevOidc2.Models.Response;
global using DevOidc2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMetadataService, MetadataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapGet("{tenantId:guid}/.well-known/openid-configuration",
    async (Guid tenantId, IMetadataService metadataService, HttpContext context) =>
    {
        var (metadata, _) = await metadataService.GetMetadataAsync(tenantId);

        if (metadata is null)
        {
            return Results.NotFound();
        }

        var baseUri = context.GetBaseUri(tenantId);

        return Results.Ok(new MetadataResponseModel
        {
            TokenEndpoint = $"{baseUri}/token",
            TokenEndpointAuthMethodsSupported = ["client_secret_post", "private_key_jwt", "client_secret_basic"],
            JwksUri = $"{baseUri}/discovery/keys",
            ResponseModesSupported = ["fragment", "form_post", "query"],
            SubjectTypesSupported = ["pairwise"],
            IdTokenSigningAlgValuesSupported = ["RS256"],
            ResponseTypesSupported = ["code", "id_token", "code id_token", "token id_token", "token"],
            ScopesSupported = ["openid"],
            Issuer = baseUri,
            AuthorizationEndpoint = $"{baseUri}/authorize",
            ClaimsSupported = metadata.SupportedClaims,
            TenantRegionScope = "EU",
            EndSessionEndpoint = $"{baseUri}/logout",
            HttpLogoutSupported = true,
            UserinfoEndpoint = $"{baseUri}/oidc/userinfo"
        });
    }).CacheOutput(builder => builder.Expire(TimeSpan.FromHours(1)));

app.MapGet("{tenantId:guid}/discovery/keys",
    async (Guid tenantId, IMetadataService metadataService, HttpContext context) =>
    {
        var (publicKey, _) = await metadataService.GetPublicKey(tenantId);

        if (publicKey is null)
        {
            return Results.NotFound();
        }

        var baseUri = context.GetBaseUri(tenantId);

        return Results.Ok(new KeysResponseModel
        {
            Keys = [new()
            {
                KeyType = publicKey.KeyType,
                Algorithm = publicKey.Algorithm,
                Use = publicKey.Use,
                Id = publicKey.Id,
                Modulus = publicKey.Modulus,
                Exponent = publicKey.Exponent,
                Issuer = baseUri
            }]
        });

    }).CacheOutput(builder => builder.Expire(TimeSpan.FromMinutes(1)));

app.Run();
