namespace DevOidc2.Models.Response;

public class MetadataResponseModel
{
    [JsonPropertyName("token_endpoint")]
    public string? TokenEndpoint { get; set; }

    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public string[]? TokenEndpointAuthMethodsSupported { get; set; }

    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; set; }

    [JsonPropertyName("response_modes_supported")]
    public string[]? ResponseModesSupported { get; set; }

    [JsonPropertyName("subject_types_supported")]
    public string[]? SubjectTypesSupported { get; set; }

    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public string[]? IdTokenSigningAlgValuesSupported { get; set; }

    [JsonPropertyName("response_types_supported")]
    public string[]? ResponseTypesSupported { get; set; }

    [JsonPropertyName("scopes_supported")]
    public string[]? ScopesSupported { get; set; }

    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }

    [JsonPropertyName("userinfo_endpoint")]
    public string? UserinfoEndpoint { get; set; }

    [JsonPropertyName("authorization_endpoint")]
    public string? AuthorizationEndpoint { get; set; }

    [JsonPropertyName("http_logout_supported")]
    public bool HttpLogoutSupported { get; set; }

    [JsonPropertyName("end_session_endpoint")]
    public string? EndSessionEndpoint { get; set; }

    [JsonPropertyName("claims_supported")]
    public string[]? ClaimsSupported { get; set; }

    [JsonPropertyName("tenant_region_scope")]
    public string? TenantRegionScope { get; set; }
}
